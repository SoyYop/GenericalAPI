using GenericalAPI.Application.Abstractions.Mappings;
using GenericalAPI.Application.Abstractions.Persistence;
using GenericalAPI.Domain.Abstractions;
using GenericalAPI.Shared.Contracts.Pagination;

namespace GenericalAPI.Application.Services;

public sealed class CrudService<TDto, TEntity> : ICrudService<TDto, TEntity>
    where TEntity : class, IAuditableEntity, new()
{
    private readonly ICrudRepository<TEntity> _repository;
    private readonly IEntityMapper<TEntity, TDto> _mapper;

    public CrudService(ICrudRepository<TEntity> repository, IEntityMapper<TEntity, TDto> mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<TDto?> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? default : _mapper.ToDto(entity);
    }

    public async Task<PagedResult<TDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetPagedAsync(request, cancellationToken);

        return new PagedResult<TDto>
        {
            Items = entities.Items.Select(_mapper.ToDto).ToList(),
            Page = entities.Page,
            PageSize = entities.PageSize,
            TotalItems = entities.TotalItems,
            TotalPages = entities.TotalPages
        };
    }

    public async Task<long> CreateAsync(TDto dto, string user, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.FromDto(dto);
        entity.CreatedBy = user;
        entity.CreatedAtUtc = DateTime.UtcNow;

        await _repository.AddAsync(entity, cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(long id, TDto dto, string user, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Entity {typeof(TEntity).Name} with id {id} was not found.");

        _mapper.MapToEntity(dto, existing);
        existing.ModifiedBy = user;
        existing.ModifiedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(existing, cancellationToken);
    }

    public Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default) =>
        _repository.DeleteAsync(id, cancellationToken);
}
