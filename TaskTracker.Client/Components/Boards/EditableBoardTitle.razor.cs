using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace TaskTracker.Client.Components.Boards;

public partial class EditableBoardTitle : ComponentBase
{
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public bool IsEditing { get; set; }
    [Parameter] public EventCallback<bool> IsEditingChanged { get; set; }
    [Parameter] public bool IsSaving { get; set; }
    [Parameter] public EventCallback<string> OnSave { get; set; }

    private string EditValue = string.Empty;
    private Input<string>? InputRef;

    protected override void OnParametersSet()
    {
        if (IsEditing && EditValue != Title)
        {
            EditValue = Title;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (IsEditing && InputRef != null)
        {
            await InputRef.Focus();
        }
    }

    private async Task StartEdit()
    {
        EditValue = Title;
        await IsEditingChanged.InvokeAsync(true);
    }

    private async Task SaveTitle()
    {
        if (string.IsNullOrWhiteSpace(EditValue))
        {
            await CancelEdit();
            return;
        }

        if (EditValue.Trim() != Title)
        {
            await OnSave.InvokeAsync(EditValue.Trim());
        }
        else
        {
            await IsEditingChanged.InvokeAsync(false);
        }
    }

    private async Task CancelEdit()
    {
        EditValue = Title;
        await IsEditingChanged.InvokeAsync(false);
    }

    private async Task HandleKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await SaveTitle();
        }
        else if (e.Key == "Escape")
        {
            await CancelEdit();
        }
    }
}
