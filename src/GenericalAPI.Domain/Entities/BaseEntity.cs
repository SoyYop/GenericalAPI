using GenericalAPI.Domain.Abstractions;

namespace GenericalAPI.Domain.Entities;

public abstract class BaseEntity : IAuditableEntity
{
    public long Id { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedAtUtc { get; set; }
    public string? ModifiedBy { get; set; }
    public Guid? CorrelationId { get; set; } = Guid.NewGuid();
}
