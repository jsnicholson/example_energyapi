using Api.Models;
using Api.Models.MeterReading;
using Api.Services.Interfaces;
using AutoMapper;
using Data.Entities;
using Data.Repositories.Interfaces;
using Data.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class MeterReadingController : ControllerBase {
        private readonly ILogger<MeterReadingController> _logger;
        private readonly ICsvService _csvService;
        private readonly IMapper _mapper;
        private readonly IMeterReadingRepository _meterReadingRepository;
        private readonly IValidationService _validationService;

        public MeterReadingController(
            ILogger<MeterReadingController> logger,
            ICsvService csvService,
            IMapper mapper,
            IMeterReadingRepository meterReadingRepository,
            IValidationService validationService) {
            _logger = logger;
            _csvService = csvService;
            _mapper = mapper;
            _meterReadingRepository = meterReadingRepository;
            _validationService = validationService;
        }

        // would rename this to just upload
        [HttpPost("meter-reading-uploads")]
        public async Task<ActionResult<UploadMeterReadingResponse>> Upload(IFormFile fileMeterReadings) {
            if (fileMeterReadings == null || fileMeterReadings.Length == 0)
                return BadRequest("Csv must have data");

            using (var reader = new StreamReader(fileMeterReadings.OpenReadStream())) {
                string fileContent = await reader.ReadToEndAsync();
                _logger.LogInformation($"{HttpContext.Request.Path.Value} received: {fileContent}");
            }

            var rawData = _csvService.Read<UploadMeterReadingRequest>(fileMeterReadings, out int countFailedToRead);
            var meterReadings = _mapper.Map<IEnumerable<MeterReading>>(rawData);
            _validationService.FilterMeterReadingsViolatingPrimaryKey(ref meterReadings);
            _validationService.FilterMeterReadingsNonExistingAccount(ref meterReadings);
            _validationService.FilterOldMeterReadings(ref meterReadings);
            var countSuccessfullyInserted = await _meterReadingRepository.CreateAsync(meterReadings);

            var response = new UploadMeterReadingResponse() {
                countSucceeded = countSuccessfullyInserted,
                countFailed = rawData.Count + countFailedToRead - countSuccessfullyInserted 
            };
            _logger.LogInformation($"{HttpContext.Request.Path.Value} responding:{JsonSerializer.Serialize(response)}");
            return new OkObjectResult(response);
        }

        [HttpGet("newest/{accountId}")]
        public async Task<ActionResult<MeterReading>> GetNewest([FromRoute] int accountId) {
            var reading = _meterReadingRepository.GetNewestAsync(accountId);

            if (reading != null)
                return new OkObjectResult(reading);
            else
                return new BadRequestObjectResult("Either account or meter readings did not exist");
        }
    }
}