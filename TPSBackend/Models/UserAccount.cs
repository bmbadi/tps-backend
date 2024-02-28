namespace TPSBackend.Models;

public class UserAccount
{
    public long UserAccountId { get; set; }
    public User User { get; set; }
    public long UserId { get; set; }
    public string AccountNumber { get; set; }
    public string AccountName { get; set; }
    public UserBalance UserBalance { get; set; }
    public ICollection<UserTransaction> UserTransactionsFrom { get; set; }
    public ICollection<UserTransaction> UserTransactionsTo { get; set; }
}