namespace TaskTracker.Application.DTOs.Pagination;

public class PagedRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 6;

    public PagedRequest()
    {
    }

    public PagedRequest(int page, int pageSize)
    {
        Page = page < 1 ? 1 : page;
        PageSize = pageSize < 1 ? 6 : (pageSize > 100 ? 100 : pageSize);
    }

    public int Skip => (Page - 1) * PageSize;
}
