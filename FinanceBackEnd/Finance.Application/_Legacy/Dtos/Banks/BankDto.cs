using Finance.Application.Legacy.Dtos.Base;

namespace Finance.Application.Legacy.Dtos.Banks;

public record BankDto : Dto<Guid>
{
    public BankDto() { }

    public string Name { get; set; } = string.Empty;
}
