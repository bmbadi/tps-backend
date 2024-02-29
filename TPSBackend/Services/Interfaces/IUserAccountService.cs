using TPSBackend.Models;

namespace TPSBackend.Services.Interfaces;

public interface IUserAccountService
{
    Task<UserAccount?> GetUserAccountByAccountNumber(string accountNumber);
    Task<UserAccount?> GetUserAccountByAccountNumberAndUser(string accountNumber, long userId);
    Task<bool> SaveUserAccount(UserAccount userAccount);
}