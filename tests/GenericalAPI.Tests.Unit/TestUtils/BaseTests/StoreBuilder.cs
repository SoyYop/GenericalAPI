using GenericalAPI.Application.Contracts.Stores;
using GenericalAPI.Domain.Entities;

namespace GenericalAPI.Tests.Unit.TestUtils.BaseTests;

public sealed class StoreBuilder : TestDataBuilder<Store>
{
    private string _name = "Store";
    private readonly List<Product> _products = [];

    public StoreBuilder WithName(string name) => With<StoreBuilder>(b => b._name = name);

    public StoreBuilder WithProduct(Product product) => With<StoreBuilder>(b => b._products.Add(product));

    public StoreBuilder WithProducts(IEnumerable<Product> products) => With<StoreBuilder>(b =>
    {
        b._products.Clear();
        b._products.AddRange(products);
    });

    public StoreBuilder FromDto(StoreDto dto) => With<StoreBuilder>(b =>
    {
        b._name = dto.Name;
        b._products.Clear();
        foreach (var p in dto.Products)
        {
            b._products.Add(new ProductBuilder().FromDto(p).Build());
        }
    });

    public override Store Build() => new()
    {
        Name = _name,
        Products = _products.ToList(),
        CreatedBy = "builder",
        CreatedAtUtc = DateTime.UtcNow
    };
}
