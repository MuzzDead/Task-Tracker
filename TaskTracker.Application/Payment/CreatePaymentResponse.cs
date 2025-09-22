namespace TaskTracker.Application.Payment;

public class CreatePaymentResponse
{
    public string SessionId { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}