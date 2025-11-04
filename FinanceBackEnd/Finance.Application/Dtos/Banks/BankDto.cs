using Finance.Application.Dtos.Base;

namespace Finance.Application.Dtos.Banks;

public record BankDto : Dto<Guid>
{
    public BankDto() { }

    public string Name { get; set; } = string.Empty;
}
