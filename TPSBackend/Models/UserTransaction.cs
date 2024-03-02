using TPSBackend.Enums;

namespace TPSBackend.Models;

public class UserTransaction
{
    public long TransactionId { get; set; }
    public User User { get; set; }
    public long UserId { get; set; }
    public TransactionType TransactionType { get; set; }
    public double Amount { get; set; }
    public UserAccount? AccountFrom { get; set; } //null for Account deposit
    public long? AccountFromId { get; set; } //null for Account deposit
    public UserAccount? AccountTo { get; set; } //null for ATM withdrawal
    public long? AccountToId { get; set; } //null for ATM withdrawal
    public double BalanceBefore { get; set; }
    public double BalanceAfter { get; set; }
    public DateTime TransactedAt { get; set; }
    
    public ICollection<AtmTransaction> AtmTransactions { get; set; }
}