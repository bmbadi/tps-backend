using TPSBackend.Enums;

namespace TPSBackend.Models;

public class User
{
    public long UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserRole UserRole { get; set; }
    public ICollection<AtmTransaction> AtmTransactions { get; set; }
    public ICollection<UserAccount> UserAccounts { get; set; }
    public ICollection<UserTransaction> UserTransactions { get; set; }
}