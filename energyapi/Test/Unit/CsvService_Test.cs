using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Globalization;
using System.Text;

namespace Test.Unit
{
    public class CsvService_Test
    {
        [Fact]
        public void CsvService_Read_AllValid()
        {
            string csvContent = "AccountId,MeterReadingDateTime,MeterReadValue,\r\n1111,22/04/2019 09:24,1002,\r\n2233,22/04/2019 10:23,323,\r\n8766,22/04/2019 12:00,999";
            var exampleExpectedMeterReading = new UploadMeterReadingRequest()
            {
                AccountId = 1111,
                MeterReadingDateTime = DateTime.ParseExact("22/04/2019 09:24", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                MeterReadValue = 1002
            };
            var fileBytes = Encoding.UTF8.GetBytes(csvContent);
            var stream = new MemoryStream(fileBytes);
            IFormFile csv = new FormFile(stream, 0, fileBytes.Length, "file", "testReadings.csv")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/csv"
            };
            var logger = new Mock<ILogger<CsvService>>();

            var csvService = new CsvService(logger.Object);

            var result = csvService.Read<UploadMeterReadingRequest>(csv, out int countFailedToRead);

            Assert.Equal(3, result.Count);
            Assert.Equal(0, countFailedToRead);
            Assert.Contains(result, r =>
                r.AccountId == exampleExpectedMeterReading.AccountId
                && r.MeterReadingDateTime == exampleExpectedMeterReading.MeterReadingDateTime
                && r.MeterReadValue == exampleExpectedMeterReading.MeterReadValue);
        }

        [Fact]
        public void CsvService_Read_AllInvalid()
        {
            string csvContent = "AccountId,MeterReadingDateTime,MeterReadValue,\r\n2233,22/04/2019 1f:25,323,\r\n8766,22/04/2019 12:00,fff";
            var fileBytes = Encoding.UTF8.GetBytes(csvContent);
            var stream = new MemoryStream(fileBytes);
            IFormFile csv = new FormFile(stream, 0, fileBytes.Length, "file", "testReadings.csv")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/csv"
            };
            var logger = new Mock<ILogger<CsvService>>();

            var csvService = new CsvService(logger.Object);

            var result = csvService.Read<UploadMeterReadingRequest>(csv, out int countFailedToRead);

            Assert.Equal(0, result.Count);
            Assert.Equal(2, countFailedToRead);
        }
    }
}