using AntDesign;
using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Column;

namespace TaskTracker.Client.Components.Columns;

public partial class CreateColumn
{
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
    [Parameter] public EventCallback<CreateColumnDto> OnColumnCreated { get; set; }
    [Parameter] public Guid BoardId { get; set; }

    private CreateColumnModel model = new();
    private bool isLoading = false;

    private FormValidationRule[] titleRules = new FormValidationRule[]
    {
            new() { Required = true, Message = "Column title is required" },
            new() { Min = 1, Message = "Title must be at least 1 character" },
            new() { Max = 100, Message = "Title cannot exceed 100 characters" }
    };

    private async Task HandleCreateColumn()
    {
        if (string.IsNullOrWhiteSpace(model.Title))
        {
            return;
        }

        isLoading = true;
        try
        {
            var createDto = new CreateColumnDto
            {
                Title = model.Title.Trim(),
                BoardId = BoardId
            };

            await OnColumnCreated.InvokeAsync(createDto);
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
        model = new CreateColumnModel();
        await IsVisibleChanged.InvokeAsync(false);
    }

    protected override void OnParametersSet()
    {
        if (!IsVisible)
        {
            model = new CreateColumnModel();
        }
    }

    private class CreateColumnModel
    {
        public string Title { get; set; } = string.Empty;
    }
}
