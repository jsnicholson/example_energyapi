 using Api.Controllers;
using Api.Mapping;
using Api.Services;
using AutoMapper;
using Data.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Test.Mocks;

namespace Test.Integration {
    public class MeterReadingController_Test {
        [Fact]
        public void MeterReadingController_Upload_AllValid_Succeeds() {
            var loggerController = new Mock<ILogger<MeterReadingController>>();
            var loggerCsvService = new Mock<ILogger<CsvService>>();
            var csvService = new CsvService(loggerCsvService.Object);
            var mapperConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new MappingProfile());
            });
            var mapper = mapperConfig.CreateMapper();
            var meterRepository = new MockMeterReadingRepository();
            var accountRepository = new MockAccountRepository();
            var loggerValidationService = new Mock<ILogger<ValidationService>>();
            var validationService = new ValidationService(loggerValidationService.Object, meterRepository, accountRepository);
            var controller = new MeterReadingController(loggerController.Object, csvService, mapper, meterRepository, validationService);

            //var result = controller.Upload()
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