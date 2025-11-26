using GenericalAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GenericalAPI.Tests.Unit.TestUtils;

/// <summary>
/// Fixture que crea una fábrica de AppDbContext para pruebas.
/// Permite alternar InMemory/LocalDb/SqlCe a través de un parámetro de entorno TEST_DB_PROVIDER o por código.
/// </summary>
public sealed class AppDbContextFixture : IAsyncDisposable
{
    public IDbContextFactory<AppDbContext> Factory { get; }
    public TestDatabaseProvider Provider { get; }

    public AppDbContextFixture(TestDatabaseProvider? providerOverride = null, string? databaseName = null)
    {
        Provider = providerOverride ?? GetProviderFromEnvironment();
        Factory = TestDatabaseHelper.CreateFactory(Provider, databaseName, unique: true);
    }

    public AppDbContext CreateContext() => Factory.CreateDbContext();

    private static TestDatabaseProvider GetProviderFromEnvironment()
    {
        var value = Environment.GetEnvironmentVariable("TEST_DB_PROVIDER");
        return value?.ToLowerInvariant() switch
        {
            "localdb" => TestDatabaseProvider.LocalDb,
            "sqlce" => TestDatabaseProvider.SqlCe,
            _ => TestDatabaseProvider.InMemory
        };
    }

    public async ValueTask DisposeAsync()
    {
        if (Factory is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
    }
}
