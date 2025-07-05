namespace Finance.Application.Dtos.Base;

public abstract record AuditedEntityDto<TId> : Dto<TId>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    protected AuditedEntityDto()
    {
    }
}
