using TPSBackend.Enums;

namespace TPSBackend.Models;

public class AtmTransaction
{
    public long TransactionId { get; set; }
    public double Amount { get; set; }
    public Atm Atm { get; set; }
    public TransactionType TransactionType { get; set; }
    public User TransactedBy { get; set; }
    public DateTime TransactedAt { get; set; }
    public UserTransaction? UserTransaction { get; set; } //for withdrawals from a user account
}