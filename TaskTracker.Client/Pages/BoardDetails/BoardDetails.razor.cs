using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Column;
using TaskTracker.Client.DTOs.Comment;
using TaskTracker.Client.DTOs.Member;
using TaskTracker.Client.DTOs.User;
using TaskTracker.Client.Services.Interfaces;
using TaskTracker.Client.States;

namespace TaskTracker.Client.Pages.BoardDetails
{
    public partial class BoardDetails : ComponentBase
    {
        [Parameter] public Guid boardId { get; set; }

        [Inject] private IBoardPageService BoardPageService { get; set; } = default!;
        [Inject] private ICardModalService CardModalService { get; set; } = default!;
        [Inject] private IAuthStateService AuthStateService { get; set; } = default!;
        [Inject] private IBoardRoleService BoardRoleService { get; set; } = default!;
        [Inject] private IUserService UserService { get; set; } = default!;

        private BoardPageState _boardState = BoardPageState.Loading();
        private CardModalState _cardModalState = CardModalState.Hidden();

        private List<MemberDto> _members = new();
        private bool _membersDrawerVisible = false;
        private bool _inviteModalVisible = false;
        private bool _isMembersLoading = false;
        private string _membersErrorMessage = string.Empty;

        private InviteStep _currentInviteStep = InviteStep.Search;
        private string _searchEmail = "";
        private string _searchError = "";
        private UserDto? _foundUser = null;
        private UserRole _selectedRole = UserRole.Member;
        private bool _isSearching = false;
        private bool _isSendingInvite = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadBoardData();
            await LoadMembers();
        }
        protected override async Task OnParametersSetAsync()
        {
            if (boardId != Guid.Empty)
            {
                await LoadBoardData();
                await LoadMembers();
            }
        }

        private async Task LoadBoardData()
        {
            _boardState = BoardPageState.Loading();
            StateHasChanged();

            _boardState = await BoardPageService.LoadBoardAsync(boardId);
            StateHasChanged();
        }

        private async Task SaveBoardTitle(string newTitle)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                return;

            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                Console.Error.WriteLine("User not authenticated");
                return;
            }

            _boardState.IsTitleSaving = true;
            StateHasChanged();

            try
            {
                var success = await BoardPageService.UpdateBoardTitleAsync(boardId, newTitle, currentUserId);
                if (success)
                {
                    _boardState.UpdateBoardTitle(newTitle);
                    _boardState.IsTitleEditing = false;
                }
            }
            finally
            {
                _boardState.IsTitleSaving = false;
                StateHasChanged();
            }
        }

        private Task OnTitleEditingChanged(bool isEditing)
        {
            _boardState.IsTitleEditing = isEditing;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private void ShowColumnModal()
        {
            _boardState.IsColumnModalVisible = true;
            StateHasChanged();
        }

        private Task HideColumnModal()
        {
            _boardState.IsColumnModalVisible = false;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task HandleColumnCreated(CreateColumnDto dto)
        {
            var success = await BoardPageService.CreateColumnAsync(dto.BoardId, dto.Title);
            if (success)
            {
                await LoadBoardData();
            }

            _boardState.IsColumnModalVisible = false;
            StateHasChanged();
        }

        private async Task OnColumnDelete(ColumnDto column)
        {
            if (_boardState.IsColumnDeleting) return;

            _boardState.IsColumnDeleting = true;
            StateHasChanged();

            try
            {
                var success = await BoardPageService.DeleteColumnAsync(column.Id);
                if (success)
                {
                    _boardState.RemoveColumn(column.Id);
                }
            }
            finally
            {
                _boardState.IsColumnDeleting = false;
                StateHasChanged();
            }
        }

        private async Task OnColumnEdit(ColumnDto column)
        {
            Console.WriteLine($"Edit column: {column.Title}");
            await Task.CompletedTask;
        }

        private async Task OnAddCard((string title, Guid columnId) data)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                Console.Error.WriteLine("User not authenticated");
                return;
            }

            var success = await BoardPageService.CreateCardAsync(data.columnId, data.title, currentUserId);
            if (success)
            {
                var updatedCards = await BoardPageService.ReloadCardsForColumnAsync(data.columnId);
                _boardState.UpdateCardsForColumn(data.columnId, updatedCards);
                StateHasChanged();
            }
        }

        private async Task OnCardClick(CardDto card)
        {
            _cardModalState = CardModalState.WithCard(card);
            StateHasChanged();

            var cardDetails = await CardModalService.LoadCardDetailsAsync(card.Id);
            _cardModalState.SetComments(cardDetails.Comments);
            StateHasChanged();
        }

        private Task HideCardDetailsModal()
        {
            _cardModalState = CardModalState.Hidden();
            StateHasChanged();
            return Task.CompletedTask;
        }

        private Task OnCardTitleEditingChanged(bool isEditing)
        {
            _cardModalState.IsTitleEditing = isEditing;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task SaveCardTitle(string newTitle)
        {
            if (_cardModalState.SelectedCard == null || string.IsNullOrWhiteSpace(newTitle))
                return;

            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                Console.Error.WriteLine("User not authenticated");
                return;
            }

            _cardModalState.IsTitleSaving = true;
            StateHasChanged();

            try
            {
                var card = _cardModalState.SelectedCard;
                var success = await CardModalService.UpdateCardTitleAsync(
                    card.Id, newTitle, card.ColumnId, card.RowIndex, currentUserId);

                if (success)
                {
                    _cardModalState.UpdateCardTitle(newTitle);

                    if (_boardState.CardsByColumn.TryGetValue(card.ColumnId, out var columnCards))
                    {
                        var cardIndex = columnCards.FindIndex(c => c.Id == card.Id);
                        if (cardIndex >= 0)
                        {
                            columnCards[cardIndex].Title = newTitle;
                        }
                    }

                    _cardModalState.IsTitleEditing = false;
                }
            }
            finally
            {
                _cardModalState.IsTitleSaving = false;
                StateHasChanged();
            }
        }

        private async Task OnCardDelete(Guid cardId)
        {
            if (_cardModalState.IsCardDeleting) return;

            _cardModalState.IsCardDeleting = true;
            StateHasChanged();

            try
            {
                var success = await CardModalService.DeleteCardAsync(cardId);
                if (success)
                {
                    _boardState.RemoveCardFromColumn(cardId);

                    if (_cardModalState.SelectedCard?.Id == cardId)
                    {
                        await HideCardDetailsModal();
                    }
                }
            }
            finally
            {
                _cardModalState.IsCardDeleting = false;
                StateHasChanged();
            }
        }

        private async Task OnCommentSubmit(string commentContent)
        {
            if (_cardModalState.SelectedCard == null || string.IsNullOrWhiteSpace(commentContent))
                return;

            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                Console.Error.WriteLine("User not authenticated");
                return;
            }

            _cardModalState.IsCommentSubmitting = true;
            StateHasChanged();

            try
            {
                var username = AuthStateService.CurrentUser?.Username ?? "Unknown";
                var commentId = await CardModalService.CreateCommentAsync(
                    _cardModalState.SelectedCard.Id, commentContent, currentUserId, username);

                if (commentId != Guid.Empty)
                {
                    var newComment = new CommentDto
                    {
                        Id = commentId,
                        Text = commentContent,
                        CardId = _cardModalState.SelectedCard.Id,
                        UserId = currentUserId,
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = username
                    };

                    _cardModalState.AddComment(newComment);
                }
            }
            finally
            {
                _cardModalState.IsCommentSubmitting = false;
                StateHasChanged();
            }
        }

        private async Task OnCommentEdit((Guid commentId, string newContent) data)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                Console.Error.WriteLine("User not authenticated");
                return;
            }

            var success = await CardModalService.UpdateCommentAsync(data.commentId, data.newContent, currentUserId);
            if (success)
            {
                _cardModalState.UpdateComment(data.commentId, data.newContent);
                StateHasChanged();
            }
        }

        private async Task OnCommentDelete(Guid commentId)
        {
            var success = await CardModalService.DeleteCommentAsync(commentId);
            if (success)
            {
                _cardModalState.RemoveComment(commentId);
                StateHasChanged();
            }
        }

        private async Task LoadMembers()
        {
            if (boardId == Guid.Empty)
            {
                Console.WriteLine("BoardId is empty, skipping member loading");
                return;
            }

            _isMembersLoading = true;
            _membersErrorMessage = string.Empty;
            StateHasChanged();

            try
            {
                var membersList = await BoardRoleService.GetMemberByBoardIdAsync(boardId);
                _members = membersList?.ToList() ?? new List<MemberDto>();
                Console.WriteLine($"Loaded {_members.Count} members for board {boardId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading members: {ex.Message}");
                _membersErrorMessage = "Failed to load members";
                _members = new List<MemberDto>();
            }
            finally
            {
                _isMembersLoading = false;
                StateHasChanged();
            }
        }

        public void OpenMembersDrawer()
        {
            _membersDrawerVisible = true;
            StateHasChanged();
        }

        private void OnMembersDrawerClose()
        {
            _membersDrawerVisible = false;
            StateHasChanged();
        }

        private void OpenInviteModal()
        {
            _inviteModalVisible = true;
            StateHasChanged();
        }

        private void CloseInviteModal()
        {
            _inviteModalVisible = false;
            ResetInviteModalState();
            StateHasChanged();
        }

        private async Task HandleEditRole((Guid BoardRoleId, UserRole NewRole) roleUpdate)
        {
            try
            {
                var updateDto = new UpdateBoardRoleDto
                {
                    Id = roleUpdate.BoardRoleId,
                    Role = roleUpdate.NewRole
                };

                await BoardRoleService.UpdateAsync(roleUpdate.BoardRoleId, updateDto);

                var member = _members.FirstOrDefault(m => m.BoardRoleId == roleUpdate.BoardRoleId);
                if (member != null)
                {
                    member.UserRole = roleUpdate.NewRole;
                    Console.WriteLine($"Updated role for member {member.Username} to {roleUpdate.NewRole}");
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating member role: {ex.Message}");
                throw;
            }
        }

        private async Task HandleRemoveMember(Guid boardRoleId)
        {
            var member = _members.FirstOrDefault(m => m.BoardRoleId == boardRoleId);
            var memberName = member?.Username ?? "Unknown";

            try
            {
                await BoardRoleService.DeleteAsync(boardRoleId);
                _members.RemoveAll(m => m.BoardRoleId == boardRoleId);
                Console.WriteLine($"Removed member {memberName} from board");
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing member {memberName}: {ex.Message}");
                throw;
            }
        }

        private async Task HandleUserSearch()
        {
            if (string.IsNullOrWhiteSpace(_searchEmail))
                return;
            _isSearching = true;
            _searchError = "";
            _foundUser = null;
            StateHasChanged();
            try
            {
                var user = await UserService.GetByEmailAsync(_searchEmail.Trim());
                if (user == null)
                {
                    _searchError = "User with this email address was not found";
                    _currentInviteStep = InviteStep.Search;
                    return;
                }
                var existingMember = _members.FirstOrDefault(m => m.UserId == user.Id);
                if (existingMember != null)
                {
                    _searchError = "This user is already a member of this board";
                    _currentInviteStep = InviteStep.Search;
                    return;
                }
                _foundUser = user;
                _currentInviteStep = InviteStep.UserFound;
                _searchError = "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching for user: {ex.Message}");
                _searchError = ex.Message.Contains("404") || ex.Message.Contains("NotFound")
                    ? "User with this email address was not found"
                    : "An error occurred while searching for the user";
                _currentInviteStep = InviteStep.Search;
            }
            finally
            {
                _isSearching = false;
                StateHasChanged();
            }
        }

        private async Task HandleSendInvite()
        {
            if (_foundUser == null)
                return;
            _isSendingInvite = true;
            StateHasChanged();
            try
            {
                var createBoardRoleDto = new CreateBoardRoleDto
                {
                    UserId = _foundUser.Id,
                    BoardId = boardId,
                    Role = _selectedRole
                };
                var member = await BoardRoleService.CreateAsync(createBoardRoleDto);
                _currentInviteStep = InviteStep.Success;
                Console.WriteLine($"Invited new member to board {boardId} with role {_selectedRole}");
                await LoadMembers();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inviting member: {ex.Message}");
                _searchError = ex.Message;
                _currentInviteStep = InviteStep.UserFound;
            }
            finally
            {
                _isSendingInvite = false;
                StateHasChanged();
            }
        }

        private void BackToSearch()
        {
            _currentInviteStep = InviteStep.Search;
            _foundUser = null;
            _searchError = "";
            _selectedRole = UserRole.Member;
            StateHasChanged();
        }

        private void ResetInviteModalState()
        {
            _currentInviteStep = InviteStep.Search;
            _searchEmail = "";
            _searchError = "";
            _foundUser = null;
            _selectedRole = UserRole.Member;
            _isSearching = false;
            _isSendingInvite = false;
        }

        private Guid GetCurrentUserId()
        {
            return AuthStateService.CurrentUser?.Id ?? Guid.Empty;
        }

        public List<MemberDto> Members => _members;
        public bool MembersDrawerVisible => _membersDrawerVisible;
        public bool InviteModalVisible => _inviteModalVisible;
        public bool IsMembersLoading => _isMembersLoading;
        public string MembersErrorMessage => _membersErrorMessage;

        public InviteStep CurrentInviteStep => _currentInviteStep;
        public string SearchEmail
        {
            get => _searchEmail;
            set { _searchEmail = value; StateHasChanged(); }
        }
        public string SearchError => _searchError;
        public UserDto? FoundUser => _foundUser;
        public UserRole SelectedRole
        {
            get => _selectedRole;
            set { _selectedRole = value; StateHasChanged(); }
        }
        public bool IsSearching => _isSearching;
        public bool IsSendingInvite => _isSendingInvite;
    }
}