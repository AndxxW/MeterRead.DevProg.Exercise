using System.ComponentModel.DataAnnotations;

namespace EnSek.MeterRead.Models.Entities
{
    public class MeterReading
    {
        [Key]
        public int ReadingId { get; set; }

        public int AccountId { get; set; }

        public DateTime MeterReadingDateTime { get; set; }

        public int MeterReadValue { get; set; }
    }
}
