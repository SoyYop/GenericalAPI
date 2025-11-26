using GenericalAPI.Application.Abstractions.Persistence;
using GenericalAPI.Application.Contracts.Stores;
using GenericalAPI.Application.Mappings.Stores;
using GenericalAPI.Application.Services;
using GenericalAPI.Domain.Entities;
using GenericalAPI.Infrastructure.Persistence;
using GenericalAPI.Infrastructure.Repositories;
using GenericalAPI.Shared.Contracts.Pagination;
using GenericalAPI.Tests.Unit.TestUtils.BaseTests;

namespace GenericalTests;

public class StoreTests : GenericRepositoryTestsBase<Store>
{
    protected override IRepository<Store> CreateRepository(AppDbContext context) => new EfStoreRepository(context);
    protected override Store CreateEntity(int seed) =>
        new StoreBuilder().WithName($"Store{seed}").Build();

    private (IStoreRepository repo, ICrudService<StoreDto, Store> svc) CreateSut(AppDbContext context)
    {
        var repo = (IStoreRepository)CreateRepository(context);
        var mapper = new StoreMapper();
        var service = new StoreService(repo, mapper);
        return (repo, service);
    }

    [Fact]
    public async Task Create_Store_With_Products_Persists_And_Get_Returns_Products()
    {
        await using var context = CreateContext();
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

    [Fact]
    public async Task GetAsync_ReturnsNull_When_NotFound()
    {
        await using var context = CreateContext();
        var (_, service) = CreateSut(context);

        var dto = await service.GetAsync(999);

        Assert.Null(dto);
    }

    [Fact]
    public async Task GetPagedAsync_Maps_Stores_To_Dtos()
    {
        await using var context = CreateContext();
        var (repo, service) = CreateSut(context);

        for (var i = 1; i <= 6; i++)
        {
            await service.CreateAsync(new StoreDto { Name = $"Shop{i}" }, user: "seed");
        }

        var result = await service.GetPagedAsync(new PagedRequest { Page = 1, PageSize = 3 });

        Assert.Equal(3, result.Items.Count);
        Assert.Equal(6, result.TotalItems);
        Assert.Equal(2, result.TotalPages);
        Assert.Contains(result.Items, s => s.Name == "Shop1");
    }

    [Fact]
    public async Task DeleteAsync_Removes_Store_And_ReturnsTrue()
    {
        await using var context = CreateContext();
        var (repo, service) = CreateSut(context);
        var id = await service.CreateAsync(new StoreDto { Name = "Temp" }, user: "seed");

        var deleted = await service.DeleteAsync(id);

        Assert.True(deleted);
        var stored = await repo.GetByIdAsync(id);
        Assert.Null(stored);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_When_NotFound()
    {
        await using var context = CreateContext();
        var (_, service) = CreateSut(context);

        var deleted = await service.DeleteAsync(123);

        Assert.False(deleted);
    }

    [Fact]
    public async Task UpdateAsync_Sets_Audit_And_Changes_Name()
    {
        await using var context = CreateContext();
        var (repo, service) = CreateSut(context);
        var id = await service.CreateAsync(new StoreDto { Name = "Old" }, user: "seed");

        await service.UpdateAsync(id, new StoreDto { Name = "New" }, user: "editor");

        var stored = await repo.GetByIdAsync(id);
        Assert.NotNull(stored);
        Assert.Equal("New", stored!.Name);
        Assert.Equal("editor", stored.ModifiedBy);
        Assert.NotNull(stored.ModifiedAtUtc);
    }

    [Fact]
    public async Task UpdateAsync_Throws_When_NotFound()
    {
        await using var context = CreateContext();
        var (_, service) = CreateSut(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.UpdateAsync(999, new StoreDto { Name = "Missing" }, user: "editor"));
    }
}
