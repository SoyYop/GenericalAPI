namespace GenericalAPI.Application.Abstractions.Services
{
    // Provides current time; inject to make time-dependent logic testable.
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
