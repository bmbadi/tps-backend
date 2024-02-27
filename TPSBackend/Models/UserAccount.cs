namespace TPSBackend.Models;

public class UserAccount
{
    public long UserAccountId { get; set; }
    public User User { get; set; }
    public string AccountNumber { get; set; }
    public string AccountName { get; set; }
}