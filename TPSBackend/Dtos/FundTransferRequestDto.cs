namespace TPSBackend.Dtos;

public class FundTransferRequestDto
{
    public string? AccountFrom { get; set; }
    public string? AccountTo { get; set; }
    public double? Amount { get; set; }
}