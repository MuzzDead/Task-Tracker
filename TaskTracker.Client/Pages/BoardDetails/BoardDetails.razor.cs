using Microsoft.AspNetCore.Components;
using Refit;
using TaskTracker.Client.DTOs.Board;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Column;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Pages.BoardDetails
{
    public partial class BoardDetails : ComponentBase
    {
        [Parameter] public Guid boardId { get; set; }

        [Inject] private IBoardService BoardService { get; set; } = default!;
        [Inject] private IColumnService ColumnService { get; set; } = default!;
        [Inject] private ICardService CardService { get; set; } = default!;
        [Inject] NavigationManager Navigation { get; set; } = default!;

        private bool _isLoading = true;
        private bool _isColumnModalVisible;

        private BoardDto? _board;
        private List<ColumnDto> _columns = new();
        private Dictionary<Guid, List<CardDto>> _cardsByColumn = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadBoardData();
        }

        private async Task LoadBoardData()
        {
            _isLoading = true;

            try
            {
                _board = await BoardService.GetByIdAsync(boardId);
                _columns = (await ColumnService.GetByBoardIdAsync(boardId)).ToList();

                var cardTasks = _columns.Select(async column => new
                {
                    ColumnId = column.Id,
                    Cards = (await CardService.GetByColumnIdAsync(column.Id)).ToList()
                });

                _cardsByColumn = (await Task.WhenAll(cardTasks))
                    .ToDictionary(r => r.ColumnId, r => r.Cards);
            }
            catch (ApiException apiEx)
            {
                Console.Error.WriteLine($"[API Error] {apiEx.StatusCode}: {apiEx.Content}");
                SetErrorState();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                SetErrorState();
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void SetErrorState()
        {
            _board = new BoardDto { Id = boardId, Title = "Error loading board" };
            _columns.Clear();
            _cardsByColumn.Clear();
        }

        private void ShowColumnModal()
        {
            _isColumnModalVisible = true;
        }

        private Task HideColumnModal()
        {
            _isColumnModalVisible = false;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task HandleColumnCreated(CreateColumnDto dto)
        {
            var nextIndex = _columns.Any() ? _columns.Max(c => c.ColumnIndex) + 1 : 0;

            var newId = await ColumnService.CreateAsync(new CreateColumnDto
            {
                Title = dto.Title,
                BoardId = dto.BoardId,
                ColumnIndex = nextIndex
            });

            var newColumn = new ColumnDto
            {
                Id = newId,
                Title = dto.Title,
                BoardId = dto.BoardId,
                ColumnIndex = nextIndex,
                CreatedAt = DateTimeOffset.Now
            };

            _columns.Add(newColumn);
            _cardsByColumn[newId] = new();
            StateHasChanged();
        }

        private async Task OnAddCard((string title, Guid columnId) data)
        {
            if (await TryCreateCard(data.title, data.columnId))
            {
                await ReloadCardsForColumn(data.columnId);
            }
        }

        private async Task<bool> TryCreateCard(string title, Guid columnId)
        {
            try
            {
                var command = new CreateCardDto { Title = title, ColumnId = columnId };
                var cardId = await CardService.CreateAsync(command);
                return cardId != Guid.Empty;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error creating card: {ex.Message}");
                return false;
            }
        }

        private async Task ReloadCardsForColumn(Guid columnId)
        {
            try
            {
                var cards = await CardService.GetByColumnIdAsync(columnId);
                _cardsByColumn[columnId] = cards.ToList();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reloading cards: {ex.Message}");
            }
        }
    }
}
