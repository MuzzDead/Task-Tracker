using TaskTracker.Application.DTOs.Pagination;

namespace TaskTracker.Application.DTOs;

public class SearchBoardsRequest : PagedRequest
{
    public string? SearchTerm { get; set; }

    public SearchBoardsRequest()
    {
    }

    public SearchBoardsRequest(string? searchTerm, int page = 1, int pageSize = 10)
        : base(page, pageSize)
    {
        SearchTerm = searchTerm;
    }
}
