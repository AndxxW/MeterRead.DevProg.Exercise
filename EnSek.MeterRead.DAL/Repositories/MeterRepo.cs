using EnSek.MeterRead.DAL.Interfaces;
using EnSek.MeterRead.Models.Classes;
using EnSek.MeterRead.Models.Entities;

namespace EnSek.MeterRead.DAL.Repositories
{
    /// <summary>
    /// Repository to allow Meter Readings to be persisted
    /// </summary>
    public class MeterRepo : IMeterRepo
    {
        private readonly IAccountContext _accountContext;

        private readonly IMeterReadingContext _meterReadingContext;

        public MeterRepo (IAccountContext accountContext, IMeterReadingContext meterReadingContext)
        {
            _accountContext = accountContext;
            _meterReadingContext = meterReadingContext;
        }

        public async Task<DbCommitCounts> CommitReadings(IEnumerable<MeterReading> readings)
        {
            var commitCounts = new DbCommitCounts();

            //TODO - There's a couple of improvements that can be made here:
            //      1. Group/order the readings by AccountId and aim to reduce the number of requests for existing readings
            //      2. Commit a batch of readings rather than individually
            foreach (var reading in readings)
            {
                //A reading must relate to a valid account
                bool canCommit = await _accountContext.DoesAccountExist(reading.AccountId);

                if (canCommit)
                {
                    var existingReadings = await _meterReadingContext.GetReadingsByAccountId(reading.AccountId);

                    //Avoid committing duplicate readings
                    canCommit = existingReadings.All(x => x.MeterReadValue != reading.MeterReadValue);

                    //Don't commit if there's a newer reading
                    canCommit = canCommit && existingReadings.All(x => x.MeterReadingDateTime < reading.MeterReadingDateTime);

                    if (canCommit)
                    {
                        _meterReadingContext.AddMeterReading(reading);

                        canCommit = canCommit && await _meterReadingContext.CommitChanges();
                    }
                }

                commitCounts.SuccessCount += canCommit ? 1 : 0;
                commitCounts.FailCount += canCommit ? 0 : 1;
            }

            return commitCounts;
        }
    }
}
