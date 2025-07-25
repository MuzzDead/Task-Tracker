using AntDesign;
using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Board;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Components.Boards;

public partial class CreateBoard
{
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
    [Parameter] public EventCallback<CreateBoardDto> OnBoardCreated { get; set; }
    [Inject] private IAuthStateService AuthStateService { get; set; } = default!;

    private CreateBoardDto model = new();
    private bool isLoading = false;

    private FormValidationRule[] titleRules = new FormValidationRule[]
    {
        new() { Required = true, Message = "Board title is required" },
        new() { Min = 3, Message = "Title must be at least 3 characters" },
        new() { Max = 100, Message = "Title cannot exceed 100 characters" }
    };

    private async Task HandleCreateBoard()
    {
        if (string.IsNullOrWhiteSpace(model.Title))
        {
            return;
        }

        isLoading = true;

        try
        {
            model.UserId = AuthStateService.CurrentUser?.Id ?? Guid.Empty;

            await OnBoardCreated.InvokeAsync(model);
            await CloseModal();
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task HandleCancel()
    {
        await CloseModal();
    }

    private async Task CloseModal()
    {
        model = new CreateBoardDto();
        await IsVisibleChanged.InvokeAsync(false);
    }

    protected override void OnParametersSet()
    {
        if (!IsVisible)
        {
            model = new CreateBoardDto();
        }
    }
}
