using GenericalAPI.Application.Abstractions.Persistence;
using GenericalAPI.Domain.Abstractions;
using GenericalAPI.Infrastructure.Persistence;
using GenericalAPI.Shared.Contracts.Pagination;
using GenericalAPI.Tests.Unit.TestUtils;

namespace GenericalAPI.Tests.Unit.TestUtils.BaseTests;

/// <summary>
/// Clase base para pruebas de repositorios genéricos.
/// Provee helpers para crear contexto, sembrar entidades y obtener paginación.
/// Las pruebas concretas solo implementan cómo crear el repositorio y las entidades.
/// </summary>
public abstract class GenericRepositoryTestsBase<TEntity>
    where TEntity : class, IAuditableEntity, new()
{
    /// <summary>
    /// Crea el repositorio específico (por ejemplo, EfRepository&lt;T&gt; o derivado).
    /// </summary>
    protected abstract ICrudRepository<TEntity> CreateRepository(AppDbContext context);

    /// <summary>
    /// Crea una instancia de entidad de prueba. El seed ayuda a generar datos diferenciados.
    /// </summary>
    protected abstract TEntity CreateEntity(int seed);

    /// <summary>
    /// Permite cambiar el proveedor de base de datos (por defecto InMemory).
    /// </summary>
    protected virtual TestDatabaseProvider DatabaseProvider => TestDatabaseProvider.InMemory;

    protected AppDbContext CreateContext(string? dbName = null, bool unique = true) =>
        TestDatabaseHelper.CreateContext(DatabaseProvider, dbName, unique);

    protected async Task<List<long>> SeedAsync(ICrudRepository<TEntity> repository, int count, int startSeed = 1, CancellationToken ct = default)
    {
        var ids = new List<long>(count);
        for (var i = 0; i < count; i++)
        {
            var entity = CreateEntity(startSeed + i);
            var saved = await repository.AddAsync(entity, ct);
            ids.Add(saved.Id);
        }

        return ids;
    }

    protected static PagedRequest Page(int page, int size) => new() { Page = page, PageSize = size };

    protected async Task<PagedResult<TEntity>> GetPagedAsync(ICrudRepository<TEntity> repository, int page, int size, CancellationToken ct = default) =>
        await repository.GetPagedAsync(Page(page, size), ct);
}
