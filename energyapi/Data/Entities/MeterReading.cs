using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities {
    [Table("MeterReading")]
    public class MeterReading {
        [ForeignKey("Account")]
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public DateTime MeterReadingDateTime { get; set; }
        public int MeterReadValue { get; set; }

        public (long AccountId, DateTime MeterReadingDateTime) Key() => (AccountId, MeterReadingDateTime);
    }
}