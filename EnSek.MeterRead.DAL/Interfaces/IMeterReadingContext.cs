using EnSek.MeterRead.Models.Entities;

namespace EnSek.MeterRead.DAL.Interfaces
{
    public interface IMeterReadingContext
    {
        Task<IEnumerable<MeterReading>> GetReadingsByAccountId(int accountId);

        void AddMeterReading(MeterReading meterReading);

        Task<bool> CommitChanges();
    }
}
