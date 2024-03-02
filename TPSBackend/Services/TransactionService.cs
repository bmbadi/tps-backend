using System.Transactions;
using Serilog;
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

    public async Task<bool> SaveFundsTransferRecords(UserTransaction userTransactionFrom, UserTransaction  userTransactionTo, UserAccount accountFrom, UserAccount accountTo)
    {
        using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                _context.UserTransactions.Add(userTransactionFrom);
                _context.UserTransactions.Add(userTransactionTo);
                _context.UserAccounts.Update(accountFrom);
                _context.UserAccounts.Update(accountTo);
                    
                int res = await _context.SaveChangesAsync();
                if (res == 4)
                {
                    ts.Complete();
                }
                else
                {
                    throw new Exception("An error occurred while SaveFundsTransferRecords: res = " + res);
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error saving fund transfer - disposing");
                ts.Dispose();
                return false;
            }
        }
    }
}