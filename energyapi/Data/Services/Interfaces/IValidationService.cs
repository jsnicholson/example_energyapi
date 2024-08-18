using Data.Entities;

namespace Data.Services.Interfaces {
    public interface IValidationService {
        void FilterOldMeterReadings(ref IEnumerable<MeterReading> meterReadings);
        void FilterMeterReadingsNonExistingAccount(ref IEnumerable<MeterReading> meterReadings);
        void FilterMeterReadingsViolatingPrimaryKey(ref IEnumerable<MeterReading> meterReadings);
    }
}