namespace GenericalAPI.Domain.Abstractions;

// Application now consumes the domain-defined contract; kept here to avoid breaking existing imports.
public interface IAuditableEntity
{
    long Id { get; set; }
    DateTime CreatedAtUtc { get; set; }
    string CreatedBy { get; set; }
    DateTime? ModifiedAtUtc { get; set; }
    string? ModifiedBy { get; set; }
    Guid? CorrelationId { get; set; }
}
