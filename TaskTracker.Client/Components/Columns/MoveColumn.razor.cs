using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Column;

namespace TaskTracker.Client.Components.Columns;

public partial class MoveColumn
{
    [Parameter] public ColumnDto? Column { get; set; }
    [Parameter] public List<ColumnDto>? BoardColumns { get; set; }
    [Parameter] public EventCallback<MoveColumnDto> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private bool _visible;
    private bool IsSaving { get; set; }
    private bool IsPositionUpdating { get; set; }
    private ColumnMoveFormModel _formModel = new();
    private Guid _initialBeforeColumnId;

    public void Open(ColumnDto column, List<ColumnDto> boardColumns)
    {
        Column = column;
        BoardColumns = boardColumns.OrderBy(c => c.ColumnIndex).ToList();
        InitializeFromColumn();
        _visible = true;
        StateHasChanged();
    }

    public void Close()
    {
        _visible = false;
        StateHasChanged();
    }

    private bool IsPositionSelected =>
        _formModel.BeforeColumnId != _initialBeforeColumnId;

    private void InitializeFromColumn()
    {
        if (Column is null || BoardColumns is null) return;

        _formModel.ColumnName = Column.Title;
        _formModel.BeforeColumnId = Guid.Empty;
        _initialBeforeColumnId = Guid.Empty;
    }

    private void OnPositionChanged(Guid newBeforeColumnId)
    {
        _formModel.BeforeColumnId = newBeforeColumnId;
    }

    private async Task HandleSave()
    {
        if (Column is null) return;

        IsSaving = true;
        try
        {
            var moveDto = new MoveColumnDto
            {
                ColumnId = Column.Id,
                BeforeColumnId = _formModel.BeforeColumnId
            };

            await OnSave.InvokeAsync(moveDto);
            _initialBeforeColumnId = _formModel.BeforeColumnId;
            Close();
        }
        finally
        {
            IsSaving = false;
        }
    }

    private async Task HandleCancel()
    {
        Close();
        await OnCancel.InvokeAsync();
    }

    private async Task HandleBackdropClick()
    {
        await HandleCancel();
    }
}