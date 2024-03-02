namespace TPSBackend.Dtos.Atm;

public class AtmUserWithdrawDto : AtmAdminFundsTransferDto
{
    public long? AccountNumber { get; set; }
}