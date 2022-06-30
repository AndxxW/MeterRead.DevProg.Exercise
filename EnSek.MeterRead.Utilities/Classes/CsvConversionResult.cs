namespace EnSek.MeterRead.Utilities.Classes
{
    /// <summary>
    /// Summary of converting csv to another format
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CsvConversionResult<T>
        where T : class
    {
        public int CsvRowCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }

        /// <summary>
        /// Class instances which represent the data contained in the csv
        /// </summary>
        public IEnumerable<T> Instances { get; set; } = Enumerable.Empty<T>();
    }
}
