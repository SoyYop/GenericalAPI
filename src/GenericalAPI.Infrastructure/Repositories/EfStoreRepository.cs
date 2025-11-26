using GenericalAPI.Application.Abstractions.Persistence;
using GenericalAPI.Domain.Entities;
using GenericalAPI.Shared.Contracts.Pagination;
using Microsoft.EntityFrameworkCore;

namespace GenericalAPI.Infrastructure.Repositories;

public sealed class EfStoreRepository : IStoreRepository
{
    private readonly DbContext _context;

    public EfStoreRepository(DbContext context)
    {
        _context = context;
    }

    private DbSet<Store> Set => _context.Set<Store>();

    public Task<Store?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        Set.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PagedResult<Store>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);

        var query = Set.AsNoTracking();
        var total = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        return new PagedResult<Store>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages
        };
    }

    public async Task<Store> AddAsync(Store entity, CancellationToken cancellationToken = default)
    {
        await Set.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Store entity, CancellationToken cancellationToken = default)
    {
        var tracked = Set.Local.FirstOrDefault(e => e.Id == entity.Id);
        if (tracked is not null && !ReferenceEquals(tracked, entity))
        {
            _context.Entry(tracked).State = EntityState.Detached;
        }

        Set.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await Set.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return false;

        Set.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public IQueryable<Store> Query() => Set.AsQueryable();

    public Task<Store?> GetByIdWithProductsAsync(long id, CancellationToken cancellationToken = default) =>
        Set.AsNoTracking().Include(s => s.Products).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
}
