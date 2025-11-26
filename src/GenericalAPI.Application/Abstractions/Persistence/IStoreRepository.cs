using GenericalAPI.Domain.Entities;

namespace GenericalAPI.Application.Abstractions.Persistence;

public interface IStoreRepository : IRepository<Store>
{
    Task<Store?> GetByIdWithProductsAsync(long id, CancellationToken cancellationToken = default);
}
