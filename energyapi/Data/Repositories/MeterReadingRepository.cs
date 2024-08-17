using Data.Entities;
using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;

namespace Data.Repositories {
    public class MeterReadingRepository : IMeterReadingRepository {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public MeterReadingRepository(ILogger<MeterReadingRepository> logger, ApplicationDbContext context) {
            _logger = logger;
            _context = context;
        }

        public async Task<bool> CreateAsync(MeterReading meterReading) {
            try {
                _context.MeterReadings.Add(meterReading);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"{MethodBase.GetCurrentMethod().Name} saving {nameof(MeterReading)}:{JsonSerializer.Serialize(meterReading)}");
                return true;
            } catch (DbUpdateException exception) {
                _logger.LogInformation($"{MethodBase.GetCurrentMethod().Name} failed to save {nameof(MeterReading)}:{JsonSerializer.Serialize(meterReading)}");

                if(exception.InnerException.Message.Contains("UNIQUE")
                    || exception.InnerException.Message.Contains("FOREIGN")) {
                    _context.Entry(meterReading).State = EntityState.Detached;
                    return false;
                }

                // unexpected so throw
                throw;
            }
        }

        public async Task<int> CreateAsync(IEnumerable<MeterReading> meterReadings) {
            int countSucceeded = 0;
            foreach(MeterReading meterReading in meterReadings) {
                countSucceeded += await CreateAsync(meterReading) ? 1 : 0;
            }
            return countSucceeded;
        }

        public async Task<MeterReading?> GetNewestAsync(int accountId) {
            return await _context.MeterReadings
                .Where(mr => mr.AccountId == accountId)
                .OrderByDescending(mr => mr.MeterReadingDateTime)
                .FirstOrDefaultAsync();
        }

        public IEnumerable<MeterReading?> GetNewest(IEnumerable<int> accountIds) {
            return _context.MeterReadings
                .Where(mr => accountIds.Contains(mr.AccountId))
                .GroupBy(mr => mr.AccountId)
                .Select(g => g.OrderByDescending(mr => mr.MeterReadingDateTime).FirstOrDefault())
                .ToList();
        }

        public async Task<IEnumerable<MeterReading?>> GetNewestAsync(IEnumerable<int> accountIds) {
            return await _context.MeterReadings
                .Where(mr => accountIds.Contains(mr.AccountId))
                .GroupBy(mr => mr.AccountId)
                .Select(g => g.OrderByDescending(mr => mr.MeterReadingDateTime).FirstOrDefault())
                .ToListAsync();
        }
    }
}