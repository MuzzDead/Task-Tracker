using AntDesign;
using Microsoft.AspNetCore.Components;
using Refit;
using TaskTracker.Client.DTOs.Board;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Pages.Boards
{
    public partial class Boards : ComponentBase
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IBoardService BoardService { get; set; } = default!;
        [Inject] private IAuthStateService AuthStateService { get; set; } = default!;
        [Inject] private IMessageService MessageService { get; set; } = default!;

        private string searchTerm = string.Empty;
        private bool isLoading = true;
        private int currentPage = 1;
        private int pageSize = 6;
        private int totalBoards;
        private List<BoardDto> allBoards = new();
        private List<BoardDto> currentPageBoards = new();

        private Guid currentUserId;
        private bool isCreateBoardModalVisible = false;

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
                allBoards.Insert(0, board);
                totalBoards = allBoards.Count;

                currentPage = 1;
                searchTerm = string.Empty;
                ApplyFilterAndPaging();

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

        private void CreateBoard()
        {
            ShowCreateBoardModal();
        }
    }
}
