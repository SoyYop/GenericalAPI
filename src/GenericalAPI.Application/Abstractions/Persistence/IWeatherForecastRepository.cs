namespace GenericalAPI.Application.Abstractions.Persistence
{
    // Repository contract for weather forecasts; implemented in Infrastructure.
    public interface IWeatherForecastRepository
    {
        Task<IEnumerable<WeatherForecastDto>> GetAsync(CancellationToken cancellationToken = default);
        Task AddAsync(WeatherForecastDto forecast, CancellationToken cancellationToken = default);
    }

    // DTO placeholder so the interface compiles until you add real models.
    public record WeatherForecastDto(DateOnly Date, int TemperatureC, string? Summary);
}
