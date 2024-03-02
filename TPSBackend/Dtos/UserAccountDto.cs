namespace TPSBackend.Dtos;

public class UserAccountDto
{
    public UserAccountDto(long userId, long accountNumber, string accountName, double balance)
    {
        UserId = userId;
        AccountNumber = accountNumber;
        AccountName = accountName;
        Balance = balance;
    }

    public long UserId { get; }
    public long AccountNumber { get; }
    public string AccountName { get; }
    public double Balance { get; }
}