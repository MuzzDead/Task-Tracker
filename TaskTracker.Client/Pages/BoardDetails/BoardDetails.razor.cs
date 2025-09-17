using Microsoft.AspNetCore.Components;
using TaskTracker.Client.Components.Comment;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Column;
using TaskTracker.Client.DTOs.Member;
using TaskTracker.Client.DTOs.State;
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
        [Inject] private ICommentService CommentService { get; set; } = default!;
        [Inject] private IUserService UserService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private BoardPageState _boardState = BoardPageState.Loading();
        private CardModalState _cardModalState = CardModalState.Hidden();

        private BoardManager _boardManager = default!;
        private ColumnManager _columnManager = default!;
        private CardManager _cardManager = default!;
        private MemberManager _memberManager = default!;
        private CardStateManager _cardStateManager = default!;

        public void OpenMembersDrawer() => _memberManager.OpenMembersDrawer();

        private void OnMembersDrawerClose() => _memberManager.OnMembersDrawerClose();

        private void OpenInviteModal() => _memberManager.OpenInviteModal();

        private void CloseInviteModal() => _memberManager.CloseInviteModal();

        private async Task HandleEditRole((Guid BoardRoleId, UserRole NewRole) roleUpdate) =>
            await _memberManager.HandleEditRoleAsync(roleUpdate);

        private async Task HandleRemoveMember(Guid boardRoleId) =>
            await _memberManager.HandleRemoveMemberAsync(boardRoleId);

        private async Task HandleUserSearch() => await _memberManager.HandleUserSearchAsync();

        private async Task HandleSendInvite() => await _memberManager.HandleSendInviteAsync();

        private void BackToSearch() => _memberManager.BackToSearch();

        private Guid GetCurrentUserId() => AuthStateService.CurrentUser?.Id ?? Guid.Empty;

        public List<MemberDto> Members => _memberManager.Members;

        public bool MembersDrawerVisible => _memberManager.MembersDrawerVisible;

        public bool InviteModalVisible => _memberManager.InviteModalVisible;

        public bool IsMembersLoading => _memberManager.IsMembersLoading;

        public string MembersErrorMessage => _memberManager.MembersErrorMessage;

        public InviteStep CurrentInviteStep => _memberManager.CurrentInviteStep;


        public string SearchEmail
        {
            get => _memberManager.SearchEmail;
            set => _memberManager.SearchEmail = value;
        }

        public string SearchError => _memberManager.SearchError;

        public UserDto? FoundUser => _memberManager.FoundUser;

        public UserRole SelectedRole
        {
            get => _memberManager.SelectedRole;
            set => _memberManager.SelectedRole = value;
        }

        public bool IsSearching => _memberManager.IsSearching;

        public bool IsSendingInvite => _memberManager.IsSendingInvite;


        protected override async Task OnInitializedAsync()
        {
            InitializeManagers();
            await LoadInitialData();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (boardId != Guid.Empty)
            {
                await LoadInitialData();
            }
        }

        private void InitializeManagers()
        {
            _boardManager = new BoardManager(
                BoardPageService,
                NavigationManager,
                GetCurrentUserId,
                () => _boardState,
                (state) => { _boardState = state; StateHasChanged(); });

            _columnManager = new ColumnManager(
                BoardPageService,
                () => _boardState,
                (state) => { _boardState = state; StateHasChanged(); });

            _cardManager = new CardManager(
                BoardPageService,
                CardModalService,
                GetCurrentUserId,
                AuthStateService,
                () => _boardState,
                () => _cardModalState,
                (boardState) => { _boardState = boardState; StateHasChanged(); },
                (cardState) => { _cardModalState = cardState; StateHasChanged(); },
                CommentService);

            _memberManager = new MemberManager(
                BoardRoleService,
                UserService,
                boardId,
                StateHasChanged);

            _cardStateManager = new CardStateManager(
                CardModalService,
                GetCurrentUserId,
                AuthStateService,
                () => _cardModalState,
                (cardState) => { _cardModalState = cardState; StateHasChanged(); },
                BoardRoleService,
                boardId);
        }

        private async Task LoadInitialData()
        {
            await _boardManager.LoadBoardDataAsync(boardId);
            await _memberManager.LoadMembersAsync();
        }


        private async Task SaveBoardTitle(string newTitle) =>
            await _boardManager.SaveBoardTitleAsync(boardId, newTitle);


        private async Task ArchiveBoard() =>
            await _boardManager.ArchiveBoardAsync(boardId);


        private Task OnTitleEditingChanged(bool isEditing) =>
            _boardManager.OnTitleEditingChanged(isEditing);


        private void ShowColumnModal() => _columnManager.ShowColumnModal();

        private Task HideColumnModal() => _columnManager.HideColumnModal();

        private async Task HandleColumnCreated(CreateColumnDto dto) =>
            await _columnManager.HandleColumnCreatedAsync(dto);

        private async Task OnColumnDelete(ColumnDto column) =>
            await _columnManager.OnColumnDeleteAsync(column);

        private async Task OnColumnTitleEditingChanged(Guid? columnId) =>
            await _columnManager.OnColumnTitleEditingChangedAsync(columnId);

        private async Task SaveColumnTitle((Guid ColumnId, string NewTitle) data) =>
            await _columnManager.SaveColumnTitleAsync(data);

        private async Task OnColumnEdit((Guid columnId, string newTitle) data) =>
            await _columnManager.OnColumnEditAsync(data);

        private async Task OnColumnMove(MoveColumnDto column) =>
            await _columnManager.OnColumnMoveAsync(column);


        private async Task OnAddCard((string title, Guid columnId) data) =>
            await _cardManager.OnAddCardAsync(data);

        private async Task OnCardClick(CardDto card) =>
            await _cardManager.OnCardClickAsync(card);

        private Task HideCardDetailsModal() => _cardManager.HideCardDetailsModal();

        private Task OnCardTitleEditingChanged(bool isEditing) =>
            _cardManager.OnCardTitleEditingChanged(isEditing);

        private async Task SaveCardTitle(string newTitle) =>
            await _cardManager.SaveCardTitleAsync(newTitle);

        private async Task OnCardDelete(Guid cardId) =>
            await _cardManager.OnCardDeleteAsync(cardId);

        private async Task OnCardMove(MoveCardDto card) =>
            await _cardManager.OnCardMoveAsync(card);

        private async Task OnCommentSubmit(CommentSubmissionData submissionData) =>
            await _cardManager.OnCommentSubmitAsync(submissionData);

        private async Task OnCommentSubmit(string commentContent) =>
            await _cardManager.OnCommentSubmitAsync(commentContent);

        private async Task OnCommentEdit((Guid commentId, string newContent) data) =>
            await _cardManager.OnCommentEditAsync(data);

        private async Task OnCommentDelete(Guid commentId) =>
            await _cardManager.OnCommentDeleteAsync(commentId);


        private async Task OnTaskComplete((Guid cardId, bool isCompleted) args) =>
            await _cardStateManager.OnCompleteTaskAsync(args.cardId, args.isCompleted);

        private async Task OnRemoveAssignment(Guid cardId) =>
            await _cardStateManager.RemoveAssignmentAsync(cardId);

        private async Task OnStateEdit((Guid cardId, Priority priority, DateTimeOffset? deadline) args) =>
            await _cardStateManager.OnStateEditAsync(args.cardId, args.priority, args.deadline);

        private async Task OnAssignUser(Guid userId)
        {
            if (_cardModalState.SelectedCard != null)
            {
                await _cardStateManager.AssignUserAsync(_cardModalState.SelectedCard.Id, userId);
            }
        }

        private async Task OnOpenAssignModal() =>
            await _cardStateManager.OpenAssignModalAsync();

        private Task OnCloseAssignModal()
        {
            _cardStateManager.CloseAssignModal();
            return Task.CompletedTask;
        }

        private async Task OnCardModalVisibleChanged(bool isVisible) =>
            await HideCardDetailsModal();

        private async Task OnTitleSave(string newTitle) =>
            await SaveCardTitle(newTitle);
    }
}