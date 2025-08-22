using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Comment;
using TaskTracker.Client.DTOs.Member;
using TaskTracker.Client.DTOs.State;
using TaskTracker.Client.DTOs.User;

namespace TaskTracker.Client.States;

public class CardModalState
{
    public CardDto? SelectedCard { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
    public StateDto? State { get; set; }
    public bool IsVisible { get; set; }

    public bool IsCommentsLoading { get; set; }
    public bool IsCommentSubmitting { get; set; }

    public bool IsTitleEditing { get; set; }
    public bool IsTitleSaving { get; set; }
    public bool IsDescriptionEditing { get; set; }
    public bool IsDescriptionSaving { get; set; }

    public bool IsCardDeleting { get; set; }

    public bool IsCompleted { get; set; }
    public UserDto? AssignedUser { get; set; }
    public bool IsAssigningUser { get; set; }
    public bool IsAssigneeLoading { get; set; }
    public bool IsRemovingAssignment { get; set; }
    public bool IsCurrentUserAssigned { get; set; }
    public bool IsAssignModalVisible { get; set; }

    public List<MemberDto> BoardMembers { get; set; } = new();
    public bool IsMembersLoading { get; set; }

    public static CardModalState WithCard(CardDto card) => new()
    {
        SelectedCard = card,
        IsVisible = true,
        IsCommentsLoading = true,
        IsAssigneeLoading = true
    };

    public static CardModalState Hidden() => new()
    {
        IsVisible = false,
        SelectedCard = null,
        Comments = new(),
        State = null,
        IsCompleted = false
    };


    public void SetCardStates(StateDto? states)
    {
        State = states;

        IsCompleted = states?.IsCompleted ?? false;
    }

    public void SetAssignedUser(UserDto? user, bool isCurrentUser = false)
    {
        AssignedUser = user;
        IsCurrentUserAssigned = isCurrentUser;
        IsAssigneeLoading = false;
    }

    public void RemoveAssignedUser()
    {
        AssignedUser = null;
        IsCurrentUserAssigned = false;
        IsRemovingAssignment = false;
    }
    public void SetComments(List<CommentDto> comments)
    {
        Comments = comments;
        IsCommentsLoading = false;
    }

    public void SetBoardMembers(List<MemberDto> members)
    {
        BoardMembers = members;
        IsMembersLoading = false;
    }

    public void AddComment(CommentDto comment)
    {
        Comments.Add(comment);
    }

    public void UpdateComment(Guid commentId, string newText)
    {
        var comment = Comments.FirstOrDefault(c => c.Id == commentId);
        if (comment != null)
        {
            comment.Text = newText;
        }
    }

    public void RemoveComment(Guid commentId)
    {
        Comments.RemoveAll(c => c.Id == commentId);
    }

    public void UpdateCardTitle(string newTitle)
    {
        if (SelectedCard != null)
        {
            SelectedCard.Title = newTitle;
        }
    }

    public void ResetEditingStates()
    {
        IsTitleEditing = false;
        IsTitleSaving = false;
        IsDescriptionEditing = false;
        IsDescriptionSaving = false;
    }

    public void UpdateCardPosition(Guid newColumnId, int newRowIndex)
    {
        if (SelectedCard != null)
        {
            SelectedCard.ColumnId = newColumnId;
            SelectedCard.RowIndex = newRowIndex;
        }
    }
}