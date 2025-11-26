namespace GenericalAPI.Application.Abstractions.Persistence
{
    // Unit of Work boundary for committing a set of changes as a single transaction.
    public interface IUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
