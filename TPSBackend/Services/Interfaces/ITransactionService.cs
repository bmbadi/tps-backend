using TPSBackend.Models;

namespace TPSBackend.Services.Interfaces;

public interface ITransactionService
{
    Task<bool> SaveFundsTransferRecords(UserTransaction userTransactionFrom, UserTransaction  userTransactionTo, UserAccount accountFrom, UserAccount accountTo);
}