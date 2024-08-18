using Api.Controllers;
using Api.Services;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test.Integration {
    public class MeterReadingController_Test {
        [Fact]
        public void MeterReadingController_Upload_AllValid_Succeeds() {
            var loggerController = new Mock<ILogger<MeterReadingController>>();
            var loggerCsvService = new Mock<ILogger<CsvService>>();
            var csvService = new CsvService(loggerCsvService.Object);
            //var mapper = new Mapper();
            //var controller = new MeterReadingController(loggerController.Object, csvService, );
        }

        [Fact]
        public void MeterReadingController_Upload_NoExistingAccount_Fails() {

        }

        [Fact]
        public void MeterReadingController_Upload_MeterReadingAlreadyExists_Fails() {

        }

        [Fact]
        public void MeterReadingController_Upload_NewerReadingAlreadyExists_Fails() {

        }
    }
}