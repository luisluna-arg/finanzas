namespace Finance.Domain.Models.Interfaces;

public interface IAuditedEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
