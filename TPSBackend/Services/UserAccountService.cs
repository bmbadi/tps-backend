using Microsoft.EntityFrameworkCore;
using TPSBackend.Data;
using TPSBackend.Dtos;
using TPSBackend.Models;
using TPSBackend.Services.Interfaces;

namespace TPSBackend.Services;

public class UserAccountService : IUserAccountService
{
    private readonly DataContext _context;

    public UserAccountService(DataContext context)
    {
        _context = context;
    }

    
    public async Task<UserAccount?> GetUserAccountByAccountNumber(long accountNumber)
    {
        return await _context.UserAccounts.FirstOrDefaultAsync(u => u.AccountNumber == accountNumber);
    }
    
    public async Task<UserAccount?> GetUserAccountByAccountNumberAndUser(long accountNumber, long userId)
    {
        return await _context.UserAccounts.FirstOrDefaultAsync(u => 
            u.AccountNumber == accountNumber && u.UserId == userId);
    }

    public async Task<bool> SaveUserAccount(UserAccount userAccount)
    {
        _context.UserAccounts.Add(userAccount);
        int res = await _context.SaveChangesAsync();
        return res == 1;
    }

    public async Task<bool> CreateNewUserAccount(User user)
    {
        UserAccount userAccount = new UserAccount
        {
            UserId = user.UserId,
            AccountName = user.Name, 
            Balance = 0.0,
            AccountNumber = GetNextAvailableAccountNumber()
        };

        return await SaveUserAccount(userAccount);
    }

    private long GetNextAvailableAccountNumber()
    {
        UserAccount? userAccountWithMaxAccountNumber = _context.UserAccounts
            .OrderByDescending(a => a.AccountNumber).FirstOrDefault();

        long max = userAccountWithMaxAccountNumber?.AccountNumber ?? 1000000;

        return max + 1;
    }
    
    public ICollection<UserAccountDto> GetUserAccountDtoFromUser(User user)
    {
        ICollection<UserAccount> userAccountList = _context.UserAccounts.Where(a => a.UserId == user.UserId).ToList();
        
        List<UserAccountDto> l = new List<UserAccountDto>();
        foreach (var userAccount in userAccountList)
        {
            l.Add(new UserAccountDto(userAccount.UserId, userAccount.AccountNumber, userAccount.AccountName, userAccount.Balance));
        }
        return l;
    }
}