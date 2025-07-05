namespace Finance.Application.Dtos;

public record KeyValueEntityDto<TEnum>() : AuditedEntityDto<TEnum>()
    where TEnum : struct, Enum
{
    public required string Name { get; set; }
    public override string ToString() => Name;
}