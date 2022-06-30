namespace EnSek.MeterRead.Utilities.Classes
{
    /// <summary>
    /// Settings to be applied during the conversion of csv to another format
    /// </summary>
    public class CsvSettings
    {
        public string? DateFormat { get; set; }

        public string NewLineDelimiter { get; set; } = Environment.NewLine;

        public string ColumnDelimiter { get; set; } = ",";

        /// <summary>
        /// Additional validation to check data items populated from the csv
        /// </summary>
        public Func<object, bool>? ValidateItem { get; set; }

    }
}
