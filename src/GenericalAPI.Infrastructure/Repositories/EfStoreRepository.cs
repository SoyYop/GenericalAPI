using GenericalAPI.Application.Abstractions.Persistence;
using GenericalAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GenericalAPI.Infrastructure.Repositories;

public sealed class EfStoreRepository : EfRepository<Store>, IStoreRepository
{
    public EfStoreRepository(DbContext context) : base(context) { }

    public Task<Store?> GetByIdWithProductsAsync(long id, CancellationToken cancellationToken = default) =>
        Set.AsNoTracking().Include(s => s.Products).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
}
