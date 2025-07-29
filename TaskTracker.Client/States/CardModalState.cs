using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Comment;

namespace TaskTracker.Client.States;

public class CardModalState
{
    public CardDto? SelectedCard { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
    public bool IsVisible { get; set; }

    public bool IsCommentsLoading { get; set; }
    public bool IsCommentSubmitting { get; set; }

    public bool IsTitleEditing { get; set; }
    public bool IsTitleSaving { get; set; }
    public bool IsDescriptionEditing { get; set; }
    public bool IsDescriptionSaving { get; set; }

    public bool IsCardDeleting { get; set; }

    public static CardModalState WithCard(CardDto card) => new()
    {
        SelectedCard = card,
        IsVisible = true,
        IsCommentsLoading = true
    };

    public static CardModalState Hidden() => new()
    {
        IsVisible = false,
        SelectedCard = null,
        Comments = new()
    };

    public void SetComments(List<CommentDto> comments)
    {
        Comments = comments;
        IsCommentsLoading = false;
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
}