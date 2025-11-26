using System.Runtime.InteropServices;
using GenericalAPI.Domain.Entities;
using GenericalAPI.Tests.Unit.TestUtils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GenericalAPI.Tests.Unit.TestUtils;

public class TestDatabaseHelperTests
{
    [Fact]
    public async Task CreateContext_InMemory_UsesUniqueDatabaseByDefault()
    {
        await using var ctx1 = TestDatabaseHelper.CreateContext();
        ctx1.Products.Add(new Product { Name = "P1", Price = 1, CreatedBy = "seed" });
        await ctx1.SaveChangesAsync();

        await using var ctx2 = TestDatabaseHelper.CreateContext();
        var count = await ctx2.Products.CountAsync();

        Assert.Equal(0, count); // unique db per call should not share state
    }

    [Fact]
    public async Task CreateContext_InMemory_AllowsSharedDatabaseWhenUniqueFalse()
    {
        const string dbName = "SharedDb";
        await using var ctx1 = TestDatabaseHelper.CreateContext(TestDatabaseProvider.InMemory, databaseName: dbName, unique: false);
        ctx1.Products.Add(new Product { Name = "P1", Price = 1, CreatedBy = "seed" });
        await ctx1.SaveChangesAsync();

        await using var ctx2 = TestDatabaseHelper.CreateContext(TestDatabaseProvider.InMemory, databaseName: dbName, unique: false);
        var count = await ctx2.Products.CountAsync();

        Assert.Equal(1, count); // same db name reused when unique=false
    }

    [Fact]
    public void CreateFactory_ReturnsFactoryAndCreatesContext()
    {
        var factory = TestDatabaseHelper.CreateFactory();
        using var ctx = factory.CreateDbContext();

        Assert.NotNull(ctx);
        Assert.Equal("Microsoft.EntityFrameworkCore.InMemory", ctx.Database.ProviderName);
    }

    [Fact]
    public void CreateContext_LocalDb_SucceedsWhenAvailable()
    {
        if (!IsLocalDbAvailable())
        {
            return; // skip when LocalDB is not present or OS not Windows
        }

        using var context = TestDatabaseHelper.CreateContext(TestDatabaseProvider.LocalDb, databaseName: "HelperLocalDbTest", unique: true);
        Assert.True(context.Database.CanConnect());
    }

    private static bool IsLocalDbAvailable()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return false;
        }

        try
        {
            using var connection = new SqlConnection("Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;Connection Timeout=2");
            connection.Open();
            return true;
        }
        catch
        {
            return false;
        }
    }

    [Fact]
    public void SqlCe_Throws_NotSupportedUntilProviderAdded()
    {
        var ex = Assert.Throws<NotSupportedException>(() =>
            TestDatabaseHelper.CreateContext(TestDatabaseProvider.SqlCe));
        Assert.Contains("SQL CE support requires adding", ex.Message);
    }
}
