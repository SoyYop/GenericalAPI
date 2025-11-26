using GenericalAPI.Application.Abstractions.Mappings;
using GenericalAPI.Application.Abstractions.Persistence;
using GenericalAPI.Application.Contracts.Stores;
using GenericalAPI.Domain.Entities;
using GenericalAPI.Shared.Contracts.Pagination;

namespace GenericalAPI.Application.Services;

public sealed class StoreService : ICrudService<StoreDto, Store>
{
    private readonly IStoreRepository _repository;
    private readonly IEntityMapper<Store, StoreDto> _mapper;

    public StoreService(IStoreRepository repository, IEntityMapper<Store, StoreDto> mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<StoreDto?> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdWithProductsAsync(id, cancellationToken);
        return entity is null ? default : _mapper.ToDto(entity);
    }

    public async Task<PagedResult<StoreDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetPagedAsync(request, cancellationToken);

        return new PagedResult<StoreDto>
        {
            Items = entities.Items.Select(_mapper.ToDto).ToList(),
            Page = entities.Page,
            PageSize = entities.PageSize,
            TotalItems = entities.TotalItems,
            TotalPages = entities.TotalPages
        };
    }

    public async Task<long> CreateAsync(StoreDto dto, string user, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.FromDto(dto);
        entity.CreatedBy = user;
        entity.CreatedAtUtc = DateTime.UtcNow;

        await _repository.AddAsync(entity, cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(long id, StoreDto dto, string user, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Entity {typeof(Store).Name} with id {id} was not found.");

        _mapper.MapToEntity(dto, existing);
        existing.ModifiedBy = user;
        existing.ModifiedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(existing, cancellationToken);
    }

    public Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default) =>
        _repository.DeleteAsync(id, cancellationToken);
}
