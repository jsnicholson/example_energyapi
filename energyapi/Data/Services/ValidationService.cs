using Data.Entities;
using Data.Repositories.Interfaces;
using Data.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Data.Services {
    public class ValidationService : IValidationService {
        private readonly ILogger _logger;
        private readonly IMeterReadingRepository _meterReadingRepository;
        private readonly IAccountRepository _accountRepository;

        public ValidationService(ILogger<ValidationService> logger, IMeterReadingRepository meterReadingRepository, IAccountRepository accountRepository) {
            _logger = logger;
            _meterReadingRepository = meterReadingRepository;
            _accountRepository = accountRepository;
        }

        public void FilterMeterReadingsViolatingPrimaryKey(ref IEnumerable<MeterReading> meterReadings) {
            var filteredReadings = meterReadings.Where(mr => !_meterReadingRepository.Exists(mr));
            _logger.LogInformation($"Removed {meterReadings.Count() - filteredReadings.Count()} entries from {nameof(meterReadings)} whose key already existed in database");
            meterReadings = filteredReadings;
        }

        public void FilterMeterReadingsNonExistingAccount(ref IEnumerable<MeterReading> meterReadings) {
            var filteredReadings = meterReadings.Where(mr => _accountRepository.Read(mr.AccountId) != null);
            _logger.LogInformation($"Removed {meterReadings.Count() - filteredReadings.Count()} entries from {nameof(meterReadings)} whose account didnt exist");
            meterReadings = filteredReadings;
        }

        public void FilterOldMeterReadings(ref IEnumerable<MeterReading> meterReadings) {
            var accountIds = meterReadings.Select(mr => mr.AccountId).Distinct();

            var newestReadingsInDb = _meterReadingRepository.GetNewest(accountIds);

            // remove readings for accounts that have newer readings in db
            var filteredReadings = meterReadings
                .Where(mr => {
                    return !newestReadingsInDb.Any(nr => nr.AccountId == mr.AccountId && nr.MeterReadingDateTime > mr.MeterReadingDateTime);
                });
            // remove readings for accounts that dont have readings already, but that have newer readings in the requested list
            filteredReadings = filteredReadings
                .GroupBy(mr => mr.AccountId)
                .Select(group => group.OrderByDescending(mr => mr.MeterReadingDateTime).First())
                .ToList();

            _logger.LogInformation($"Removed {meterReadings.Count() - filteredReadings.Count()} older entries from {nameof(meterReadings)}");

            meterReadings = filteredReadings;
        }
    }
}