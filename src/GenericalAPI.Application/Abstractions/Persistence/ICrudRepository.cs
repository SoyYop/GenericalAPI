using GenericalAPI.Domain.Abstractions;
using GenericalAPI.Shared.Contracts.Pagination;

namespace GenericalAPI.Application.Abstractions.Persistence;


public interface ICrudRepository<TEntity>
    where TEntity : IAuditableEntity
{
    Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<PagedResult<TEntity>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
    IQueryable<TEntity> Query();
}
