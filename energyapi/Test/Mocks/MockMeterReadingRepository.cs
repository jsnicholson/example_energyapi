using Data.Entities;
using Data.Repositories.Interfaces;

namespace Test.Mocks {
    public class MockMeterReadingRepository : IMeterReadingRepository {
        private IDictionary<(int accountId, DateTime meterReadingDateTime), MeterReading> _meterReadings = new Dictionary<(int accountId, DateTime meterReadingDateTime), MeterReading>();

        public async Task<bool> CreateAsync(MeterReading meterReading) {
            _meterReadings.Add((accountId: meterReading.AccountId, meterReadingDateTime: meterReading.MeterReadingDateTime), meterReading);
            return true;
        }

        public async Task<int> CreateAsync(IEnumerable<MeterReading> meterReadings) {
            int count = 0;
            foreach(var meterReading in meterReadings) {
                var success = await CreateAsync(meterReading);
                count += (success) ? 1 : 0;
            }

            return count;
        }

        public bool Exists(int accountId, DateTime meterReadingDateTime) {
            return _meterReadings.ContainsKey((accountId, meterReadingDateTime));
        }

        public bool Exists(MeterReading meterReading) {
            return _meterReadings.ContainsKey((meterReading.AccountId, meterReading.MeterReadingDateTime));
        }

        public IEnumerable<MeterReading?> GetNewest(IEnumerable<int> accountIds) {
            var filteredMeterReadings = _meterReadings
                .Where(kv => accountIds.Contains(kv.Key.accountId))
                .Select(kv => kv.Value);

            if (filteredMeterReadings.Any()) return new List<MeterReading?>();

            return filteredMeterReadings
                .GroupBy(mr => mr.AccountId)
                .Select(g => g.OrderByDescending(mr => mr.MeterReadingDateTime).FirstOrDefault());
        }

        public async Task<MeterReading?> GetNewestAsync(int accountId) {
            var filteredMeterReadings = _meterReadings
                .Where(kv => kv.Key.accountId == accountId)
                .Select(kv => kv.Value);

            if (!filteredMeterReadings.Any()) return null;

            return filteredMeterReadings
                .OrderByDescending(mr => mr.MeterReadingDateTime)
                .FirstOrDefault();
        }
    }
}