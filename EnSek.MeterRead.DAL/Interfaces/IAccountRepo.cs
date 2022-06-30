namespace EnSek.MeterRead.DAL.Interfaces
{
    public interface IAccountRepo
    {
        Task<bool> DoesAccountExist(int accountId);
    }
}
