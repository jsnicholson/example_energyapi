using Data.Entities;
using Data.Repositories.Interfaces;
using Data.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test.Unit
{
    public class ValidationService_Test
    {
        [Fact]
        public void ValidationService_FilterOldMeterReadings_OnlyNewRemaining()
        {
            // expecting to see only one 123 remaining (2 passed in but one older)
            // expecting to see only one 222 remaining (newer one in db)
            IEnumerable<MeterReading> newMeterReadings = new List<MeterReading>() {
                new() {
                    AccountId = 123,
                    MeterReadingDateTime = DateTime.Parse("2024-08-01 12:00:00"),
                    MeterReadValue = 1000
                },
                new() {
                    AccountId = 123,
                    MeterReadingDateTime = DateTime.Parse("2024-08-01 11:00:00"),
                    MeterReadValue = 1001
                },
                new() {
                    AccountId = 222,
                    MeterReadingDateTime = DateTime.Parse("2024-08-01 11:00:00"),
                    MeterReadValue = 900
                }
            };
            IEnumerable<MeterReading> existingMeterReadings = [
                new() {
                    AccountId = 222,
                    MeterReadingDateTime = DateTime.Parse("2024-08-01 12:00:00"),
                    MeterReadValue = 800
                }
            ];

            var logger = new Mock<ILogger<ValidationService>>();
            var repository = new Mock<IMeterReadingRepository>();
            repository.Setup(x => x.GetNewest(It.IsAny<IEnumerable<int>>()))
                .Returns(existingMeterReadings);
            var accountRepository = new Mock<IAccountRepository>();

            var validationService = new ValidationService(logger.Object, repository.Object, accountRepository.Object);

            validationService.FilterOldMeterReadings(ref newMeterReadings);

            // ensure only one for that account
            Assert.Single(newMeterReadings.Where(r => r.AccountId == 123));
            // ensure remaining has hour 12 (ie. correct one removed)
            Assert.Equal(12, newMeterReadings.First().MeterReadingDateTime.Hour);
            // should be none remaining for account 222 as newer in db
            Assert.Empty(newMeterReadings.Where(r => r.AccountId == 222));
        }
    }
}