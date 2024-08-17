using Data.Entities;

namespace Data.Services.Interfaces {
    public interface IValidationService {
        void FilterOldMeterReadings(ref IEnumerable<MeterReading> meterReadings);
    }
}