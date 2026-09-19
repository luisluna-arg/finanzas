using Finance.Application.Dtos.Base;

namespace Finance.Application.Dtos.CreditCards;

public record CreditCardInstallmentPatternDto : Dto<Guid>
{
    public CreditCardInstallmentPatternDto() { }

    public string Name { get; set; } = string.Empty;
    public string RegexPattern { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
    public Guid? UserId { get; set; }
}
