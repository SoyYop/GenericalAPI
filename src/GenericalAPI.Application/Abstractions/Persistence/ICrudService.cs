using GenericalAPI.Domain.Abstractions;
using GenericalAPI.Shared.Contracts.Pagination;

namespace GenericalAPI.Application.Abstractions.Persistence;

public interface ICrudService<TDto, TEntity>
    where TEntity : IAuditableEntity, new()
{
    Task<TDto?> GetAsync(long id, CancellationToken cancellationToken = default);
    Task<PagedResult<TDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<long> CreateAsync(TDto dto, string user, CancellationToken cancellationToken = default);
    Task UpdateAsync(long id, TDto dto, string user, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
