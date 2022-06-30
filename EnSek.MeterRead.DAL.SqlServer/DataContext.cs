using EnSek.MeterRead.DAL.Interfaces;
using EnSek.MeterRead.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnSek.MeterRead.DAL.SqlServer
{
    /// <summary>
    /// Using a code first strategy to generate the database
    /// </summary>
    public class DataContext : DbContext, IAccountContext, IMeterReadingContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Account>? Accounts { get; set; }

        public DbSet<MeterReading>? MeterReadings { get; set; }

        public async Task<bool> DoesAccountExist(int accountId)
        {
            return await Accounts!.FirstOrDefaultAsync(x => x.AccountId == accountId) != null;
        }
        public async Task<IEnumerable<MeterReading>> GetReadingsByAccountId(int accountId)
        {
            return await MeterReadings!.Where(x => x.AccountId == accountId).ToListAsync();
        }

        public void AddMeterReading(MeterReading meterReading)
        {
            MeterReadings?.Add(meterReading);
        }

        public async Task<bool> CommitChanges()
        {
            return await SaveChangesAsync() > 0;
        }
    }
}
