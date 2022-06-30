namespace EnSek.MeterRead.Models.Dtos
{
    public class MeterReadingsUploadsSummaryDto
    {
        public int CommitSuccessCount { get; set; }

        public int CommitFailureCount { get; set; }

        public int InvalidCsvRowCount { get; set; }
    }
}
