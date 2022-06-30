using EnSek.MeterRead.Models.Classes;
using EnSek.MeterRead.Models.Entities;

namespace EnSek.MeterRead.DAL.Interfaces
{
    public interface IMeterRepo
    {
        public Task<DbCommitCounts> CommitReadings(IEnumerable<MeterReading> readings);
    }
}
