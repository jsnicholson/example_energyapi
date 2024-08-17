using Data.Entities;
using Data.Repositories.Interfaces;
using Data.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Data.Services {
    public class ValidationService : IValidationService {
        private readonly ILogger _logger;
        private readonly IMeterReadingRepository _meterReadingRepository;

        public ValidationService(ILogger<ValidationService> logger, IMeterReadingRepository meterReadingRepository) {
            _logger = logger;
            _meterReadingRepository = meterReadingRepository;
        }

        public void FilterOldMeterReadings(ref IEnumerable<MeterReading> meterReadings) {
            var accountIds = meterReadings.Select(mr => mr.AccountId).Distinct();

            var newestReadingsInDb = _meterReadingRepository.GetNewest(accountIds);

            var filteredReadings = meterReadings
                .Where(mr => {
                    return newestReadingsInDb.Any(nr => nr.AccountId == mr.AccountId && nr.MeterReadingDateTime < mr.MeterReadingDateTime);
                });
            _logger.LogInformation($"Removed {meterReadings.Count() - filteredReadings.Count()} entries from {nameof(meterReadings)}");

            meterReadings = filteredReadings;
        }
    }
}