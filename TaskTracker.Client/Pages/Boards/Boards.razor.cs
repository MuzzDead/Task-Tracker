using AntDesign;
using Microsoft.AspNetCore.Components;
using Refit;
using TaskTracker.Client.DTOs.Board;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Pages.Boards;

public partial class Boards : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IBoardService BoardService { get; set; } = default!;

    private string searchTerm = string.Empty;

    private bool isLoading = true;
    private int currentPage = 1;
    private int pageSize = 6;
    private int totalBoards;
    private List<BoardDto> allBoards = new();
    private List<BoardDto> currentPageBoards = new();

    private Guid currentUserId = new("c7dc3069-4e5e-f011-8d15-f09e4af2796a");

    protected override async Task OnInitializedAsync()
    {
        await LoadBoardsAsync();
    }

    private async Task LoadBoardsAsync()
    {
        isLoading = true;
        try
        {
            allBoards = await BoardService.GetByUserIdAsync(currentUserId);
            totalBoards = allBoards.Count;
            ApplyFilterAndPaging();
        }
        catch (ApiException ex)
        {
            Console.Error.WriteLine($"Refit error while fetching boards: {ex.StatusCode} - {ex.Content}");
            allBoards = new();
        }
        finally
        {
            isLoading = false;
        }
    }

    private void ApplyFilterAndPaging()
    {
        var filtered = string.IsNullOrWhiteSpace(searchTerm)
            ? allBoards
            : allBoards.Where(b =>
                    b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (b.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();

        totalBoards = filtered.Count;
        currentPageBoards = filtered
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    private void OnSearchChangedValue(string newValue)
    {
        searchTerm = newValue;
        currentPage = 1;
        ApplyFilterAndPaging();
    }

    private async Task OnPageChange(PaginationEventArgs args)
    {
        currentPage = args.Page;
        ApplyFilterAndPaging();
        await InvokeAsync(StateHasChanged);
    }

    private void CreateBoard()
    {
        NavigationManager.NavigateTo("/boards/create");
    }

    private Task OpenBoard(Guid boardId)
    {
        NavigationManager.NavigateTo($"/boards/{boardId}");
        return Task.CompletedTask;
    }
}