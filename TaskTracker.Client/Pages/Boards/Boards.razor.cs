using AntDesign;
using Microsoft.AspNetCore.Components;
using Refit;
using TaskTracker.Client.DTOs.Board;
using TaskTracker.Client.DTOs.Pagination;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Pages.Boards;

public partial class Boards : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IBoardService BoardService { get; set; } = default!;
    [Inject] private IAuthStateService AuthStateService { get; set; } = default!;
    [Inject] private IMessageService MessageService { get; set; } = default!;

    private bool isLoading = true;
    private int currentPage = 1;
    private int pageSize = 6;
    private int totalBoards;
    private List<BoardDto> currentPageBoards = new();

    private Guid currentUserId;
    private bool isCreateBoardModalVisible = false;

    private string searchTerm = string.Empty;
    private string lastSearchTerm = string.Empty;
    private bool isSearching = false;

    protected override async Task OnInitializedAsync()
    {
        await AuthStateService.InitializeAsync();

        if (AuthStateService.CurrentUser == null)
        {
            NavigationManager.NavigateTo("/login");
            return;
        }

        currentUserId = AuthStateService.CurrentUser.Id;
        await LoadBoardsAsync();
    }

    private async Task LoadBoardsAsync()
    {
        isLoading = true;
        try
        {
            var result = await BoardService.GetByUserIdAsync(currentUserId, currentPage, pageSize);
            currentPageBoards = result.Items;
            totalBoards = result.TotalCount;
        }
        catch (ApiException ex)
        {
            Console.Error.WriteLine($"Error loading boards: {ex.StatusCode} - {ex.Content}");
            currentPageBoards = new();
            totalBoards = 0;
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task SearchBoardsAsync()
    {
        if (searchTerm == lastSearchTerm) return;

        isSearching = true;
        lastSearchTerm = searchTerm;
        currentPage = 1; 

        try
        {
            PagedResult<BoardDto> result;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                result = await BoardService.GetByUserIdAsync(currentUserId, currentPage, pageSize);
            }
            else
            {
                result = await BoardService.SearchAsync(searchTerm, currentPage, pageSize);
            }

            currentPageBoards = result.Items;
            totalBoards = result.TotalCount;
        }
        catch (ApiException ex)
        {
            Console.Error.WriteLine($"Search error: {ex.StatusCode} - {ex.Content}");
            MessageService.Error("Failed to search boards. Please try again.");
            currentPageBoards = new();
            totalBoards = 0;
        }
        finally
        {
            isSearching = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task OnPageChange(PaginationEventArgs args)
    {
        if (currentPage == args.Page) return;

        currentPage = args.Page;

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            await LoadBoardsAsync();
        }
        else
        {
            await SearchBoardsAsync();
        }

        await InvokeAsync(StateHasChanged);
    }

    private void ShowCreateBoardModal()
    {
        isCreateBoardModalVisible = true;
    }

    private async Task OnCreateBoardModalVisibilityChanged(bool isVisible)
    {
        isCreateBoardModalVisible = isVisible;
        await InvokeAsync(StateHasChanged);
    }

    private async Task HandleBoardCreated(CreateBoardDto createBoardDto)
    {
        try
        {
            var newBoard = await BoardService.CreateAsync(createBoardDto);
            var board = await BoardService.GetByIdAsync(newBoard);

            currentPage = 1;
            searchTerm = string.Empty;
            lastSearchTerm = string.Empty;
            await LoadBoardsAsync();

            MessageService.Success($"Board '{board.Title}' created successfully!");
            await InvokeAsync(StateHasChanged);
        }
        catch (ApiException ex)
        {
            var errorMessage = ex.StatusCode switch
            {
                System.Net.HttpStatusCode.BadRequest => "Invalid board data. Please check your input.",
                System.Net.HttpStatusCode.Unauthorized => "You are not authorized to create boards.",
                System.Net.HttpStatusCode.Conflict => "A board with this title already exists.",
                _ => $"Failed to create board: {ex.Content}"
            };

            MessageService.Error(errorMessage);
            Console.Error.WriteLine($"Board creation error: {ex.StatusCode} - {ex.Content}");
        }
        catch (Exception ex)
        {
            MessageService.Error("An unexpected error occurred. Please try again.");
            Console.Error.WriteLine($"Unexpected error creating board: {ex}");
        }
    }

    private Task OpenBoard(Guid boardId)
    {
        NavigationManager.NavigateTo($"/boards/{boardId}");
        return Task.CompletedTask;
    }

    private async Task OnSearchClick(string searchValue)
    {
        searchTerm = searchValue;
        await SearchBoardsAsync();
    }

    private void CreateBoard()
    {
        ShowCreateBoardModal();
    }
}
