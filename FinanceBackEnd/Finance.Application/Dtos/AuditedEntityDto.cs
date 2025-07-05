namespace Finance.Application.Dtos;

public abstract record AuditedEntityDto<TId> : Dto<TId>
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
