using GenericalAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GenericalAPI.Tests.Unit;

public enum TestDatabaseProvider
{
    InMemory,
    LocalDb
}

public static class TestDatabaseHelper
{
    public static AppDbContext CreateContext(
        TestDatabaseProvider provider = TestDatabaseProvider.InMemory,
        string? databaseName = null,
        bool unique = true)
    {
        var options = BuildOptions(provider, databaseName, unique);
        var context = new AppDbContext(options);

        if (provider == TestDatabaseProvider.LocalDb)
        {
            context.Database.EnsureCreated();
        }

        return context;
    }

    public static IDbContextFactory<AppDbContext> CreateFactory(
        TestDatabaseProvider provider = TestDatabaseProvider.InMemory,
        string? databaseName = null,
        bool unique = true)
    {
        var options = BuildOptions(provider, databaseName, unique);
        var factory = new SimpleDbContextFactory(options);

        if (provider == TestDatabaseProvider.LocalDb)
        {
            using var context = factory.CreateDbContext();
            context.Database.EnsureCreated();
        }

        return factory;
    }

    private static DbContextOptions<AppDbContext> BuildOptions(
        TestDatabaseProvider provider,
        string? databaseName,
        bool unique)
    {
        var dbName = ComposeDatabaseName(databaseName, unique);
        var builder = new DbContextOptionsBuilder<AppDbContext>();

        return provider switch
        {
            TestDatabaseProvider.LocalDb => builder
                .UseSqlServer(GetLocalDbConnectionString(dbName))
                .Options,
            _ => builder
                .UseInMemoryDatabase(dbName)
                .Options
        };
    }

    private static string ComposeDatabaseName(string? baseName, bool unique)
    {
        var name = string.IsNullOrWhiteSpace(baseName)
            ? "GenericApiTests"
            : baseName.Trim();

        return unique ? $"{name}_{Guid.NewGuid():N}" : name;
    }

    private static string GetLocalDbConnectionString(string databaseName) =>
        $"Server=(localdb)\\mssqllocaldb;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true";

    private sealed class SimpleDbContextFactory(DbContextOptions<AppDbContext> options)
        : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext() => new(options);
    }
}
