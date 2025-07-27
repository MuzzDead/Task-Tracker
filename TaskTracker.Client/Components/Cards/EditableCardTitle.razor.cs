using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace TaskTracker.Client.Components.Cards;

public partial class EditableCardTitle : ComponentBase
{
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public bool IsEditing { get; set; }
    [Parameter] public bool IsSaving { get; set; }
    [Parameter] public EventCallback<bool> OnTitleEditingChanged { get; set; }
    [Parameter] public EventCallback<string> OnTitleSave { get; set; }

    private string EditValue = string.Empty;
    private Input<string>? InputRef;

    protected override void OnParametersSet()
    {
        if (!IsEditing)
        {
            EditValue = Title;
        }
    }

    private async Task StartEdit()
    {
        if (IsSaving) return;

        EditValue = Title;
        await OnTitleEditingChanged.InvokeAsync(true);
        await Task.Delay(50);
        if (InputRef != null)
            await InputRef.Focus();
    }

    private async Task SaveTitle()
    {
        if (string.IsNullOrWhiteSpace(EditValue)) return;

        await OnTitleSave.InvokeAsync(EditValue.Trim());
    }

    private async Task CancelEdit()
    {
        if (IsSaving) return;

        EditValue = Title;
        await OnTitleEditingChanged.InvokeAsync(false);
    }

    private async Task HandleKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !string.IsNullOrWhiteSpace(EditValue))
        {
            await SaveTitle();
        }
        else if (e.Key == "Escape")
        {
            await CancelEdit();
        }
    }
}
