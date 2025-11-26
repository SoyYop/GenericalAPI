namespace GenericalAPI.Application.Contracts.Stores;

public sealed class StoreDto
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public IReadOnlyCollection<ProductDto> Products { get; init; } = Array.Empty<ProductDto>();
}

public sealed class ProductDto
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string? Description { get; init; }
    public long? StoreId { get; init; }
}
