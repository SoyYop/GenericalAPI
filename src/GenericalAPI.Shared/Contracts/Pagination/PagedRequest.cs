namespace GenericalAPI.Shared.Contracts.Pagination;

public sealed class PagedRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? OrderBy { get; init; }
    public bool Desc { get; init; }
}
