namespace GenericalAPI.Domain.Entities;

public sealed class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    // Optional relation to Store
    public long? StoreId { get; set; }
    public Store? Store { get; set; }
}
