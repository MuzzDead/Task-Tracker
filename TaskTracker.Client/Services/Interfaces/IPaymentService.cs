using Refit;
using TaskTracker.Client.DTOs.Payment;

namespace TaskTracker.Client.Services.Interfaces;

public interface IPaymentService
{
    [Post("/payment/create-checkout-session")]
    Task<CreatePaymentResponse> CreateCheckoutSession([Body] CreatePaymentRequest request);
}
