using TPSBackend.Models;

namespace TPSBackend.Services.Interfaces;

public interface ITransactionService
{
    Task<bool> SaveUserTransaction(UserTransaction userTransaction);
}