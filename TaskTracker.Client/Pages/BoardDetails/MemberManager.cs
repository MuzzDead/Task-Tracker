using TaskTracker.Client.DTOs.Member;
using TaskTracker.Client.DTOs.User;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Pages.BoardDetails;

public class MemberManager
{
    private readonly IBoardRoleService _boardRoleService;
    private readonly IUserService _userService;
    private readonly Guid _boardId;
    private readonly Action _stateHasChanged;

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

    public List<MemberDto> Members => _members;
    public bool MembersDrawerVisible => _membersDrawerVisible;
    public bool InviteModalVisible => _inviteModalVisible;
    public bool IsMembersLoading => _isMembersLoading;
    public string MembersErrorMessage => _membersErrorMessage;
    public InviteStep CurrentInviteStep => _currentInviteStep;
    public string SearchEmail
    {
        get => _searchEmail;
        set { _searchEmail = value; _stateHasChanged(); }
    }
    public string SearchError => _searchError;
    public UserDto? FoundUser => _foundUser;
    public UserRole SelectedRole
    {
        get => _selectedRole;
        set { _selectedRole = value; _stateHasChanged(); }
    }
    public bool IsSearching => _isSearching;
    public bool IsSendingInvite => _isSendingInvite;
    public MemberManager(
        IBoardRoleService boardRoleService,
        IUserService userService,
        Guid boardId,
        Action stateHasChanged)
    {
        _boardRoleService = boardRoleService;
        _userService = userService;
        _boardId = boardId;
        _stateHasChanged = stateHasChanged;
    }

    public async Task LoadMembersAsync()
    {
        if (_boardId == Guid.Empty)
        {
            Console.WriteLine("BoardId is empty, skipping member loading");
            return;
        }

        _isMembersLoading = true;
        _membersErrorMessage = string.Empty;
        _stateHasChanged();

        try
        {
            var membersList = await _boardRoleService.GetMemberByBoardIdAsync(_boardId);
            _members = membersList?.ToList() ?? new List<MemberDto>();
            Console.WriteLine($"Loaded {_members.Count} members for board {_boardId}");
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
            _stateHasChanged();
        }
    }

    public void OpenMembersDrawer()
    {
        _membersDrawerVisible = true;
        _stateHasChanged();
    }

    public void OnMembersDrawerClose()
    {
        _membersDrawerVisible = false;
        _stateHasChanged();
    }

    public void OpenInviteModal()
    {
        _inviteModalVisible = true;
        _stateHasChanged();
    }

    public void CloseInviteModal()
    {
        _inviteModalVisible = false;
        ResetInviteModalState();
        _stateHasChanged();
    }

    public async Task HandleEditRoleAsync((Guid BoardRoleId, UserRole NewRole) roleUpdate)
    {
        try
        {
            var updateDto = new UpdateBoardRoleDto
            {
                Id = roleUpdate.BoardRoleId,
                Role = roleUpdate.NewRole
            };

            await _boardRoleService.UpdateAsync(roleUpdate.BoardRoleId, updateDto);

            var member = _members.FirstOrDefault(m => m.BoardRoleId == roleUpdate.BoardRoleId);
            if (member != null)
            {
                member.UserRole = roleUpdate.NewRole;
                Console.WriteLine($"Updated role for member {member.Username} to {roleUpdate.NewRole}");
                _stateHasChanged();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating member role: {ex.Message}");
            throw;
        }
    }

    public async Task HandleRemoveMemberAsync(Guid boardRoleId)
    {
        var member = _members.FirstOrDefault(m => m.BoardRoleId == boardRoleId);
        var memberName = member?.Username ?? "Unknown";

        try
        {
            await _boardRoleService.DeleteAsync(boardRoleId);
            _members.RemoveAll(m => m.BoardRoleId == boardRoleId);
            Console.WriteLine($"Removed member {memberName} from board");
            _stateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing member {memberName}: {ex.Message}");
            throw;
        }
    }

    public async Task HandleUserSearchAsync()
    {
        if (string.IsNullOrWhiteSpace(_searchEmail))
            return;

        _isSearching = true;
        _searchError = "";
        _foundUser = null;
        _stateHasChanged();

        try
        {
            var user = await _userService.GetByEmailAsync(_searchEmail.Trim());
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
            _stateHasChanged();
        }
    }

    public async Task HandleSendInviteAsync()
    {
        if (_foundUser == null)
            return;

        _isSendingInvite = true;
        _stateHasChanged();

        try
        {
            var createBoardRoleDto = new CreateBoardRoleDto
            {
                UserId = _foundUser.Id,
                BoardId = _boardId,
                Role = _selectedRole
            };

            var member = await _boardRoleService.CreateAsync(createBoardRoleDto);
            _currentInviteStep = InviteStep.Success;
            Console.WriteLine($"Invited new member to board {_boardId} with role {_selectedRole}");
            await LoadMembersAsync();
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
            _stateHasChanged();
        }
    }

    public void BackToSearch()
    {
        _currentInviteStep = InviteStep.Search;
        _foundUser = null;
        _searchError = "";
        _selectedRole = UserRole.Member;
        _stateHasChanged();
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
}