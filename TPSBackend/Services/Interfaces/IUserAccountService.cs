using TPSBackend.Dtos;
using TPSBackend.Models;

namespace TPSBackend.Services.Interfaces;

public interface IUserAccountService
{
    Task<UserAccount?> GetUserAccountByAccountNumber(long accountNumber);
    Task<UserAccount?> GetUserAccountByAccountNumberAndUser(long accountNumber, long userId);
    Task<bool> SaveUserAccount(UserAccount userAccount);
    Task<bool> CreateNewUserAccount(User user);
    ICollection<UserAccountDto> GetUserAccountDtoFromUser(User user);
}