using GenericalAPI.Application.Abstractions.Persistence;
using GenericalAPI.Domain.Abstractions;
using GenericalAPI.Shared.Contracts.Pagination;
using Microsoft.EntityFrameworkCore;

namespace GenericalAPI.Infrastructure.Repositories;

public class EfRepository<TEntity> : ICrudRepository<TEntity>
    where TEntity : class, IAuditableEntity
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> Set;

    public EfRepository(DbContext context)
    {
        Context = context;
        Set = context.Set<TEntity>();
    }

    public Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        Set.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PagedResult<TEntity>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);

        var query = ApplyOrdering(Set.AsNoTracking(), request);
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
        await Set.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var tracked = Set.Local.FirstOrDefault(e => e.Id == entity.Id);
        if (tracked is not null && !ReferenceEquals(tracked, entity))
        {
            Context.Entry(tracked).State = EntityState.Detached;
        }

        Set.Update(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await Set.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        Set.Remove(entity);
        await Context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public IQueryable<TEntity> Query() => Set.AsQueryable();

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
