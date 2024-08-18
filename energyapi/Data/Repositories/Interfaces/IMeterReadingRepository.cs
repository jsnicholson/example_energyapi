using Data.Entities;

namespace Data.Repositories.Interfaces {
    public interface IMeterReadingRepository {
        /// <summary>
        /// Create a new MeterReading in the database
        /// </summary>
        /// <param name="meterReading">MeterReading to be created</param>
        /// <returns>Boolean whether created</returns>
        Task<bool> CreateAsync(MeterReading meterReading);
        /// <summary>
        /// Creates many new MeterReadings
        /// </summary>
        /// <param name="meterReadings">MeterReadings to be created</param>
        /// <returns>Count of successfully created MeterReadings</returns>
        Task<int> CreateAsync(IEnumerable<MeterReading> meterReadings);
        /// <summary>
        /// Gets newest meter reading for given account id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>Newest MeterReading, or null if doesnt exist</returns>
        Task<MeterReading?> GetNewestAsync(int accountId);
        IEnumerable<MeterReading?> GetNewest(IEnumerable<int> accountIds);
        bool Exists(int accountId, DateTime meterReadingDateTime);
        bool Exists(MeterReading meterReading);
    }
}