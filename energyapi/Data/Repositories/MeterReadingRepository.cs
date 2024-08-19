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
            _context.MeterReadings.Add(meterReading);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"CreateAsync saving {nameof(MeterReading)}:{JsonSerializer.Serialize(meterReading)}");
            return true;
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

        public bool Exists(int accountId, DateTime meterReadingDateTime) {
            return _context.MeterReadings.Any(mr => mr.AccountId == accountId 
                && mr.MeterReadingDateTime == meterReadingDateTime);
        }

        public bool Exists(MeterReading meterReading) {
            return _context.MeterReadings.Any(mr => mr.AccountId == meterReading.AccountId 
                && mr.MeterReadingDateTime == meterReading.MeterReadingDateTime);
        }
    }
}