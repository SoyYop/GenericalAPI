using GenericalAPI.Infrastructure.Repositories;
using GenericalAPI.Infrastructure.Persistence;
using GenericalAPI.Application.Abstractions.Persistence;
using GenericalAPI.Domain.Entities;
using GenericalAPI.Application.Services;
using GenericalAPI.Application.Mappings.Products;
using GenericalAPI.Shared.Contracts.Pagination;
using GenericalAPI.Tests.Unit;
using Xunit.Abstractions;
using GenericalAPI.Application.Contracts.Stores;

namespace GenericApi.Tests;

public class CrudTests(ITestOutputHelper output)
{
    private static (IRepository<Product> repo, ICrudService<ProductDto, Product> svc) CreateSut(AppDbContext context)
    {
        var repo = new EfRepository<Product>(context);
        var mapper = new ProductMapper();
        var service = new CrudService<ProductDto, Product>(repo, mapper);
        return (repo, service);
    }

    [Fact]
    public async Task Repository_Paginates_And_Orders_By_Id()
    {
        await using var context = TestDatabaseHelper.CreateContext();
        var repo = new EfRepository<Product>(context);

        for (var i = 1; i <= 30; i++)
        {
            await repo.AddAsync(new Product
            {
                Name = $"P{i}",
                Price = i,
                CreatedBy = "seed"
            });
        }

        var result = await repo.GetPagedAsync(new PagedRequest { Page = 2, PageSize = 10 });

        Assert.Equal(10, result.Items.Count);
        Assert.Equal(2, result.Page);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(30, result.TotalItems);
        Assert.Equal(11, result.Items.First().Id); // second page starts at 11 when ordered by Id asc
    }

    [Fact]
    public async Task CrudService_Sets_Audit_On_Create_And_Update()
    {
        await using var context = TestDatabaseHelper.CreateContext();
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
}
