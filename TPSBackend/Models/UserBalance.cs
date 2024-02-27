namespace TPSBackend.Models;

public class UserBalance
{
    public long UserBalanceId { get; set; }
    public UserAccount UserAccount { get; set; }
    public double Balance { get; set; }
}