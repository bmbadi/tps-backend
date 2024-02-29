using TPSBackend.Data;
using TPSBackend.Models;
using TPSBackend.Services.Interfaces;

namespace TPSBackend.Services;

public class TransactionService : ITransactionService
{
    private readonly DataContext _context;

    public TransactionService(DataContext context)
    {
        _context = context;
    }

    public async Task<bool> SaveUserTransaction(UserTransaction userTransaction)
    {
        _context.UserTransactions.Add(userTransaction);
        int res = await _context.SaveChangesAsync();
        return res == 1;
    }
}