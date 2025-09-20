using Stripe.Checkout;

namespace TaskTracker.Application.Payment;

public interface IPaymentService
{
    Task<CreatePaymentResponse> CreateCheckoutSessionAsync(CreatePaymentRequest request,
        CancellationToken cancellationToken = default);
}