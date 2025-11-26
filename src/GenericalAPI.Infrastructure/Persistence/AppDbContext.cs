using GenericalAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GenericalAPI.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Store> Stores => Set<Store>();
}
