using TPSBackend.Enums;

namespace TPSBackend.Models;

public class UserTransaction
{
    public long TransactionId { get; set; }
    public User User { get; set; }
    public TransactionType TransactionType { get; set; }
    public double Amount { get; set; }
    public UserAccount AccountFrom { get; set; }
    public UserAccount AccountTo { get; set; }
    public double BalanceBefore { get; set; }
    public double BalanceAfter { get; set; }
    public User TransactedBy { get; set; }
    public DateTime TransactedAt { get; set; }
}