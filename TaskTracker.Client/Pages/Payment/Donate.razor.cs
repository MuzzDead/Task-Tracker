using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using TaskTracker.Client.DTOs.Payment;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Pages.Payment;

public partial class Donate : ComponentBase
{
    [Inject] private IPaymentService PaymentService { get; set; } = default!;
    [Inject] private IMessageService MessageService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;


    private DonationForm donationForm = new DonationForm();
    private bool isProcessing = false;
    private readonly decimal[] quickAmounts = { 5, 10, 25, 50, 100 };

    public class DonationForm
    {
        public decimal Amount { get; set; } = 10;
        public string Currency { get; set; } = "usd";
    }

    protected override void OnInitialized()
    {
        donationForm.Amount = 10;
    }

    private void SetQuickAmount(decimal amount)
    {
        donationForm.Amount = amount;
        StateHasChanged();
    }

    private async Task OnFinish(EditContext editContext)
    {
        isProcessing = true;
        StateHasChanged();

        try
        {
            var request = new CreatePaymentRequest
            {
                Amount = (long)(donationForm.Amount * 100)
            };

            var response = await PaymentService.CreateCheckoutSession(request);

            if (response != null)
            {
                await JSRuntime.InvokeVoidAsync("open", response.Url, "_blank");
            }
            else
            {
                MessageService.Error("Error creating payment session!");
            }
        }
        catch (Exception ex)
        {
            MessageService.Error($"An error has occurred: {ex.Message}");
        }
        finally
        {
            isProcessing = false;
            StateHasChanged();
        }
    }

    private Task OnFinishFailed(EditContext editContext)
    {
        MessageService.Error("Please check the information you have entered!");
        return Task.CompletedTask;
    }
}
