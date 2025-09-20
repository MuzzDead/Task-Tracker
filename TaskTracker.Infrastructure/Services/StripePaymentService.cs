using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;
using TaskTracker.Application.Payment;

namespace TaskTracker.Infrastructure.Services;


public class StripePaymentService : IPaymentService
{
    private readonly string _secretKey;
    private readonly string _publishableKey;
    private readonly string _succesUrl;
    private readonly string _cancelUrl;
    public StripePaymentService(IConfiguration configuration)
    {
        _secretKey = configuration["Stripe:SecretKey"] ?? throw new ArgumentNullException("Stripe:SecretKey");
        _publishableKey = configuration["Stripe:PublishableKey"] ?? throw new ArgumentNullException("Stripe:PublishableKey");
        _succesUrl = configuration["Stripe:SuccessUrl"] ?? throw new ArgumentNullException("Stripe:SuccessUrl");
        _cancelUrl = configuration["Stripe:CancelUrl"] ?? throw new ArgumentNullException("Stripe:CancelUrl");
        StripeConfiguration.ApiKey = _secretKey;
    }
    public async Task<CreatePaymentResponse> CreateCheckoutSessionAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = request.Currency,
                        UnitAmount = request.Amount,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "TaskTracker"
                        }
                    },
                    Quantity = 1
                }
            },
            Mode = "payment",
            SuccessUrl = _succesUrl,
            CancelUrl = _cancelUrl,
            Locale = "en"
        };
        var service = new SessionService();
        var session = await service.CreateAsync(options, cancellationToken: cancellationToken);
        return new CreatePaymentResponse
        {
            SessionId = session.Id,
            PublicKey = _publishableKey,
            Url = session.Url
        };
    }
}
