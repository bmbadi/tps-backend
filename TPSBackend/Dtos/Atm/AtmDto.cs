namespace TPSBackend.Dtos.Atm;

public class AtmDto
{
    public AtmDto(long atmId, double balance)
    {
        AtmId = atmId;
        Balance = balance;
    }

    public long AtmId { get; }
    public double Balance { get; }
}