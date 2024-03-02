namespace TPSBackend.Dtos;

public class FundTransferRequestDto
{
    public long? AccountFrom { get; set; }
    public long? AccountTo { get; set; }
    public double? Amount { get; set; }
}