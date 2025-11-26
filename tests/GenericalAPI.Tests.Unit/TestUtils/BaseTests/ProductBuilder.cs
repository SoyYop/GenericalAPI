using GenericalAPI.Application.Contracts.Stores;
using GenericalAPI.Domain.Entities;

namespace GenericalAPI.Tests.Unit.TestUtils.BaseTests;

public sealed class ProductBuilder : TestDataBuilder<Product>
{
    private string _name = "Product";
    private decimal _price = 1m;
    private string? _description;
    private long? _storeId;

    public ProductBuilder WithName(string name) => With<ProductBuilder>(b => b._name = name);
    public ProductBuilder WithPrice(decimal price) => With<ProductBuilder>(b => b._price = price);
    public ProductBuilder WithDescription(string? description) => With<ProductBuilder>(b => b._description = description);
    public ProductBuilder WithStoreId(long? storeId) => With<ProductBuilder>(b => b._storeId = storeId);

    public ProductBuilder FromDto(ProductDto dto) => With<ProductBuilder>(b =>
    {
        b._name = dto.Name;
        b._price = dto.Price;
        b._description = dto.Description;
        b._storeId = dto.StoreId;
    });

    public override Product Build() => new()
    {
        Name = _name,
        Price = _price,
        Description = _description,
        StoreId = _storeId,
        CreatedBy = "builder",
        CreatedAtUtc = DateTime.UtcNow
    };
}
