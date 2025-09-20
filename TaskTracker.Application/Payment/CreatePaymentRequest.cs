using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Payment;

public class CreatePaymentRequest
{
    [Required(ErrorMessage = "The amount is required")]
    [Range(1, 999999, ErrorMessage = "The amount must be between 1 and 999,999.")]
    public long Amount { get; set; }
    public string? Currency { get; set; } = "usd";
}
