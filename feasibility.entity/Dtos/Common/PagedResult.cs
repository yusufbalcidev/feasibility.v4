namespace feasibility.Entity.Dtos.Common;

public interface IPagedResult
{
    int TotalCount { get; }
    int Page { get; }
    int PageSize { get; }
    string? Search { get; }
    int TotalPages { get; }
    bool HasPrevious { get; }
    bool HasNext { get; }
    int FirstItemIndex { get; }
    int LastItemIndex { get; }
}

public class PagedResult<T> : IPagedResult
{
    public IReadOnlyList<T> Items { get; set; } = new List<T>();

    public int TotalCount { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? Search { get; set; }

    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;

    public int FirstItemIndex => TotalCount == 0 ? 0 : (Page - 1) * PageSize + 1;

    public int LastItemIndex => Math.Min(Page * PageSize, TotalCount);

    public static PagedResult<T> Create(IReadOnlyList<T> items, int totalCount, int page, int pageSize, string? search)
        => new()
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Search = search
        };
}
