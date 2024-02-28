namespace TPSBackend.Models;

public class Atm
{
    public long AtmId { get; set; }
    public double Balance { get; set; }
    public ICollection<AtmTransaction> AtmTransactions { get; set; }
}