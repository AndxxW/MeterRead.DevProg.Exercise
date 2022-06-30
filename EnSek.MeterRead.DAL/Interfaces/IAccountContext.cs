using EnSek.MeterRead.Models.Entities;

namespace EnSek.MeterRead.DAL.Interfaces
{
    public interface IAccountContext
    {
        Task<bool> DoesAccountExist(int accountId);
    }
}
