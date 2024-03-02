using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TPSBackend.Data;
using TPSBackend.Dtos.Atm;
using TPSBackend.Models;
using TPSBackend.Services.Interfaces;

namespace TPSBackend.Services;

public class AtmService : IAtmService
{
    private readonly DataContext _context;

    public AtmService(DataContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateAtm(Atm atm)
    {
        _context.Atms.Add(atm);
        int res = await _context.SaveChangesAsync();
        return res == 1;
    }

    public AtmDto AtmToAtmDto(Atm atm)
    {
        return new AtmDto(atm.AtmId, atm.Balance);
    }

    public ICollection<AtmDto> AtmToAtmDto(ICollection<Atm> atmList)
    {
        List<AtmDto> l = new List<AtmDto>();
        foreach (var atm in atmList)
        {
            l.Add(AtmToAtmDto(atm));
        }
        return l;
    }

    public async Task<ICollection<Atm>> GetAllAtms()
    {
        return await _context.Atms.ToListAsync();
    }

    public async Task<Atm?> GetAtmById(long atmId)
    {
        return await _context.Atms.FirstOrDefaultAsync(a => a.AtmId == atmId);
    }

    public async Task<bool> SaveAtmAdminTransferFundsRecords(AtmTransaction atmTransaction, Atm atm)
    {
        using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                _context.AtmTransactions.Add(atmTransaction);
                _context.Atms.Update(atm);
                    
                int res = await _context.SaveChangesAsync();
                if (res == 2)
                {
                    ts.Complete();
                }
                else
                {
                    throw new Exception("An error occurred while SaveAtmAdminDepositFundsRecords: res = " + res);
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error saving SaveAtmAdminDepositFundsRecords - disposing");
                ts.Dispose();
                return false;
            }
        }
    }
    
    public async Task<bool> SaveAtmUserWithdrawFundsRecords(AtmTransaction atmTransaction, Atm atm, UserAccount userAccount, UserTransaction userTransaction)
    {
        using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                _context.UserTransactions.Add(userTransaction);
                await _context.SaveChangesAsync();
                
                atmTransaction.UserTransactionId = userTransaction.TransactionId;
                _context.AtmTransactions.Add(atmTransaction);
                _context.Atms.Update(atm);
                _context.UserAccounts.Update(userAccount);
                    
                int res = await _context.SaveChangesAsync();
                if (res > 0)
                {
                    ts.Complete();
                }
                else
                {
                    throw new Exception("An error occurred while SaveAtmUserWithdrawFundsRecords: res = " + res);
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error saving SaveAtmUserWithdrawFundsRecords - disposing");
                ts.Dispose();
                return false;
            }
        }
    }
}