using GenericalAPI.Application.Abstractions.Persistence;
using GenericalAPI.Application.Contracts.Stores;
using GenericalAPI.Application.Mappings.Products;
using GenericalAPI.Application.Services;
using GenericalAPI.Domain.Entities;
using GenericalAPI.Infrastructure.Persistence;
using GenericalAPI.Infrastructure.Repositories;
using GenericalAPI.Shared.Contracts.Pagination;
using GenericalAPI.Tests.Unit.TestUtils.BaseTests;

namespace GenericApi.Tests;

public class CrudTests : GenericRepositoryTestsBase<Product>
{
    protected override IRepository<Product> CreateRepository(AppDbContext context) => new EfRepository<Product>(context);
    protected override Product CreateEntity(int seed) =>
        new ProductBuilder().WithName($"P{seed}").WithPrice(seed).Build();

    private (IRepository<Product> Repo, ICrudService<ProductDto, Product> Service) CreateSut(AppDbContext context)
    {
        var repo = CreateRepository(context);
        var mapper = new ProductMapper();
        var service = new CrudService<ProductDto, Product>(repo, mapper);
        return (repo, service);
    }

    [Fact]
    public async Task Repository_Paginates_And_Orders_By_Id()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        await SeedAsync(repo, 30);

        var result = await repo.GetPagedAsync(new PagedRequest { Page = 2, PageSize = 10 });

        Assert.Equal(10, result.Items.Count);
        Assert.Equal(2, result.Page);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(30, result.TotalItems);
        Assert.Equal(11, result.Items.First().Id); // segunda página inicia en 11
    }

    [Fact]
    public async Task Repository_Orders_By_Id_Descending_When_Requested()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);
        await SeedAsync(repo, 5);

        var result = await repo.GetPagedAsync(new PagedRequest { Page = 1, PageSize = 5, Desc = true });

        Assert.Equal(5, result.Items.Count);
        Assert.Equal(5, result.Items.First().Id); // con Desc=true, el primero es el Id más alto
    }

    [Fact]
    public async Task Repository_Orders_By_Name_Ascending_When_Specified()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);
        await repo.AddAsync(new ProductBuilder().WithName("Zeta").Build());
        await repo.AddAsync(new ProductBuilder().WithName("Alpha").Build());
        await repo.AddAsync(new ProductBuilder().WithName("Beta").Build());

        var result = await repo.GetPagedAsync(new PagedRequest { Page = 1, PageSize = 10, OrderBy = "Name" });

        var orderedNames = result.Items.Select(p => p.Name).ToList();
        Assert.Equal(new[] { "Alpha", "Beta", "Zeta" }, orderedNames);
    }

    [Fact]
    public async Task Repository_Orders_By_Name_Descending_When_Specified()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);
        await repo.AddAsync(new ProductBuilder().WithName("Zeta").Build());
        await repo.AddAsync(new ProductBuilder().WithName("Alpha").Build());
        await repo.AddAsync(new ProductBuilder().WithName("Beta").Build());

        var result = await repo.GetPagedAsync(new PagedRequest { Page = 1, PageSize = 10, OrderBy = "Name", Desc = true });

        var orderedNames = result.Items.Select(p => p.Name).ToList();
        Assert.Equal(new[] { "Zeta", "Beta", "Alpha" }, orderedNames);
    }

    [Fact]
    public async Task CrudService_Sets_Audit_On_Create_And_Update()
    {
        await using var context = CreateContext();
        var (repo, service) = CreateSut(context);

        var newId = await service.CreateAsync(new ProductDto
        {
            Name = "Keyboard",
            Price = 50m
        }, user: "alice");

        var created = await repo.GetByIdAsync(newId);
        Assert.NotNull(created);
        Assert.Equal("alice", created!.CreatedBy);
        Assert.NotEqual(default, created.CreatedAtUtc);
        Assert.Null(created.ModifiedBy);
        Assert.Null(created.ModifiedAtUtc);

        await service.UpdateAsync(newId, new ProductDto
        {
            Name = "Keyboard Pro",
            Price = 75m
        }, user: "bob");

        var updated = await repo.GetByIdAsync(newId);
        Assert.NotNull(updated);
        Assert.Equal("Keyboard Pro", updated!.Name);
        Assert.Equal(75m, updated.Price);
        Assert.Equal("bob", updated.ModifiedBy);
        Assert.NotNull(updated.ModifiedAtUtc);
    }

    [Fact]
    public async Task GetAsync_ReturnsDto_When_Entity_Exists()
    {
        await using var context = CreateContext();
        var (_, service) = CreateSut(context);

        var id = await service.CreateAsync(new ProductDto { Name = "Mouse", Price = 20m }, user: "seed");

        var dto = await service.GetAsync(id);

        Assert.NotNull(dto);
        Assert.Equal("Mouse", dto!.Name);
    }

    [Fact]
    public async Task GetAsync_ReturnsNull_When_NotFound()
    {
        await using var context = CreateContext();
        var (_, service) = CreateSut(context);

        var dto = await service.GetAsync(123456);

        Assert.Null(dto);
    }

    [Fact]
    public async Task GetPagedAsync_Maps_Entities_To_Dtos()
    {
        await using var context = CreateContext();
        var (repo, service) = CreateSut(context);
        await SeedAsync(repo, 10);

        var result = await service.GetPagedAsync(new PagedRequest { Page = 1, PageSize = 5 });

        Assert.Equal(5, result.Items.Count);
        Assert.Equal(10, result.TotalItems);
        Assert.Equal(2, result.TotalPages);
        Assert.Contains(result.Items, p => p.Name == "P1");
    }

    [Fact]
    public async Task DeleteAsync_Removes_Entity_And_ReturnsTrue()
    {
        await using var context = CreateContext();
        var (repo, service) = CreateSut(context);
        var id = await service.CreateAsync(new ProductDto { Name = "ToDelete", Price = 1m }, user: "seed");

        var deleted = await service.DeleteAsync(id);

        Assert.True(deleted);
        var fetched = await repo.GetByIdAsync(id);
        Assert.Null(fetched);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_When_NotFound()
    {
        await using var context = CreateContext();
        var (_, service) = CreateSut(context);

        var deleted = await service.DeleteAsync(999);

        Assert.False(deleted);
    }

    [Fact]
    public async Task UpdateAsync_Throws_When_NotFound()
    {
        await using var context = CreateContext();
        var (_, service) = CreateSut(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.UpdateAsync(42, new ProductDto { Name = "Missing", Price = 1m }, user: "tester"));
    }
}
