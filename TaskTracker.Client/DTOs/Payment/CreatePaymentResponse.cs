namespace TaskTracker.Client.DTOs.Payment;

public class CreatePaymentResponse
{
    public string SessionId { get; set; }
    public string PublicKey { get; set; }
    public string Url { get; set; }
}