using GenericalAPI.Domain.Entities;

namespace GenericalAPI.Application.Abstractions.Persistence;

public interface IStoreRepository : ICrudRepository<Store>
{
    Task<Store?> GetByIdWithProductsAsync(long id, CancellationToken cancellationToken = default);
}
