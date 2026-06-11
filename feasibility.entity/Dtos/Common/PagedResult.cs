namespace feasibility.Entity.Dtos.Common;

/// <summary>
/// Generic olmayan sayfalama bilgisi. _Pagination partial'ı bu arayüze bağlanır.
/// </summary>
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

/// <summary>
/// Sayfalanmış liste sonucu. Index sayfalarında 10'arlı sayfalama + arama için kullanılır.
/// </summary>
public class PagedResult<T> : IPagedResult
{
    public IReadOnlyList<T> Items { get; set; } = new List<T>();

    /// <summary>Filtre (arama) uygulandıktan sonraki toplam kayıt sayısı.</summary>
    public int TotalCount { get; set; }

    /// <summary>1 tabanlı geçerli sayfa.</summary>
    public int Page { get; set; } = 1;

    /// <summary>Sayfa başına kayıt sayısı (varsayılan 10).</summary>
    public int PageSize { get; set; } = 10;

    /// <summary>Aktif arama terimi (boş olabilir).</summary>
    public string? Search { get; set; }

    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;

    /// <summary>Gösterilen aralığın başlangıç sıra numarası (1 tabanlı), liste boşsa 0.</summary>
    public int FirstItemIndex => TotalCount == 0 ? 0 : (Page - 1) * PageSize + 1;

    /// <summary>Gösterilen aralığın bitiş sıra numarası.</summary>
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
