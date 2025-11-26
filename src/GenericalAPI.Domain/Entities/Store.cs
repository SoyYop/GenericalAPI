namespace GenericalAPI.Domain.Entities;

public sealed class Store : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
