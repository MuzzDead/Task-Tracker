using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Payment;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CreatePaymentRequest request)
    {
        try
        {
            var response = await _paymentService.CreateCheckoutSessionAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}