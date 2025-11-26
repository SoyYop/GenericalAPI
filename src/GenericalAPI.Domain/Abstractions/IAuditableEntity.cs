namespace GenericalAPI.Domain.Abstractions;

public interface IAuditableEntity
{
    long Id { get; set; }
    DateTime CreatedAtUtc { get; set; }
    string CreatedBy { get; set; }
    DateTime? ModifiedAtUtc { get; set; }
    string? ModifiedBy { get; set; }
    Guid? CorrelationId { get; set; }
}
