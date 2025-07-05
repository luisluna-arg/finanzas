namespace Finance.Application.Dtos.Base;

public record KeyValueEntityDto<TEnum> : AuditedEntityDto<TEnum>
    where TEnum : struct, Enum
{
    public string Name { get; set; } = string.Empty;
    public override string ToString() => Name;
    
    public KeyValueEntityDto()
    {
    }
}