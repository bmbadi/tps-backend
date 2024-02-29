using Microsoft.EntityFrameworkCore;
using TPSBackend.Data;
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

    
    public async Task<UserAccount?> GetUserAccountByAccountNumber(string accountNumber)
    {
        return await _context.UserAccounts.FirstOrDefaultAsync(u => u.AccountNumber == accountNumber);
    }
    
    public async Task<UserAccount?> GetUserAccountByAccountNumberAndUser(string accountNumber, long userId)
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
}