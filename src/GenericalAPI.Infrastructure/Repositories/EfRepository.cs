using GenericalAPI.Application.Abstractions.Persistence;
using GenericalAPI.Domain.Abstractions;
using GenericalAPI.Shared.Contracts.Pagination;
using Microsoft.EntityFrameworkCore;

namespace GenericalAPI.Infrastructure.Repositories;

public sealed class EfRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, IAuditableEntity
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _set;

    public EfRepository(DbContext context)
    {
        _context = context;
        _set = context.Set<TEntity>();
    }

    public Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        _set.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PagedResult<TEntity>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);

        var query = ApplyOrdering(_set.AsNoTracking(), request);
        var total = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        return new PagedResult<TEntity>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages
        };
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _set.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var tracked = _set.Local.FirstOrDefault(e => e.Id == entity.Id);
        if (tracked is not null && !ReferenceEquals(tracked, entity))
        {
            _context.Entry(tracked).State = EntityState.Detached;
        }

        _set.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _set.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _set.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public IQueryable<TEntity> Query() => _set.AsQueryable();

    private static IQueryable<TEntity> ApplyOrdering(IQueryable<TEntity> source, PagedRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OrderBy))
        {
            return request.Desc
                ? source.OrderByDescending(e => e.Id)
                : source.OrderBy(e => e.Id);
        }

        // Use EF.Property to allow ordering by simple property names while staying type-safe enough.
        return request.Desc
            ? source.OrderByDescending(e => EF.Property<object>(e, request.OrderBy))
            : source.OrderBy(e => EF.Property<object>(e, request.OrderBy));
    }
}
