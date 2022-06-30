namespace EnSek.MeterRead.Models.Classes
{
    /// <summary>
    /// Summary persisting entities to a database
    /// </summary>
    public class DbCommitCounts
    {
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
    }
}
