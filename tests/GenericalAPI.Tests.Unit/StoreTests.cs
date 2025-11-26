using GenericalAPI.Infrastructure.Repositories;
using GenericalAPI.Infrastructure.Persistence;
using GenericalAPI.Application.Abstractions.Persistence;
using GenericalAPI.Domain.Entities;
using GenericalAPI.Application.Services;
using GenericalAPI.Application.Contracts.Stores;
using GenericalAPI.Application.Mappings.Products;
using GenericalAPI.Application.Mappings.Stores;
using GenericalAPI.Tests.Unit;

namespace GenericalTests;

public class StoreTests
{
    private static (IStoreRepository repo, ICrudService<StoreDto, Store> svc) CreateSut(AppDbContext context)
    {
        var repo = new EfStoreRepository(context);
        var mapper = new StoreMapper();
        var service = new StoreService(repo, mapper);
        return (repo, service);
    }

    [Fact]
    public async Task Create_Store_With_Products_Persists_And_Get_Returns_Products()
    {
        await using var context = TestDatabaseHelper.CreateContext();
        var (repo, service) = CreateSut(context);

        var dto = new StoreDto
        {
            Name = "Shop",
            Products = new List<ProductDto>
            {
                new() { Name = "Item1", Price = 9.99m }
            }
        };

        var id = await service.CreateAsync(dto, user: "tester");

        var stored = await repo.GetByIdWithProductsAsync(id);
        Assert.NotNull(stored);
        Assert.Equal("tester", stored!.CreatedBy);
        Assert.Single(stored.Products);

        var fetchedDto = await service.GetAsync(id);
        Assert.NotNull(fetchedDto);
        Assert.Single(fetchedDto!.Products);
        Assert.Equal("Item1", fetchedDto.Products.First().Name);
    }
}
