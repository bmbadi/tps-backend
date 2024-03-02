using System.Collections;
using TPSBackend.Dtos.Atm;
using TPSBackend.Models;

namespace TPSBackend.Services.Interfaces;

public interface IAtmService
{
    Task<bool> CreateAtm(Atm atm);
    AtmDto AtmToAtmDto(Atm atm);
    ICollection<AtmDto> AtmToAtmDto(ICollection<Atm> atmList);
    Task<ICollection<Atm>> GetAllAtms();
    Task<Atm?> GetAtmById(long atmId);
    Task<bool> SaveAtmAdminTransferFundsRecords(AtmTransaction atmTransaction, Atm atm);
    Task<bool> SaveAtmUserWithdrawFundsRecords(AtmTransaction atmTransaction, Atm atm, UserAccount userAccount, UserTransaction userTransaction);
}