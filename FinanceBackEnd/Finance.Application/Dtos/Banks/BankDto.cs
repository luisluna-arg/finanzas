using Finance.Application.Dtos.Base;

namespace Finance.Application.Dtos.Banks;

public record BankDto : Dto<Guid>
{
    public string Name { get; set; } = string.Empty;

    public BankDto()
    {
    }
}
