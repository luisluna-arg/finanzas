namespace Finance.Application.Dtos.Banks;

public record BankDto : Dto<Guid>
{
    public BankDto()
        : base()
    {
    }

    public string Name { get; set; } = string.Empty;
}
