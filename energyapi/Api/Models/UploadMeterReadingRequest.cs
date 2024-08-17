using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Api.Models {
    public class UploadMeterReadingRequest {
        public long AccountId { get; set; }
        [Format("dd/MM/yyyy HH:mm")]
        public DateTime MeterReadingDateTime { get; set; }
        [Range(-99999,99999, ErrorMessage = "MeterReadValue must be between -99999 and 99999")]
        public int MeterReadValue { get; set; }
    }
}