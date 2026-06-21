using Finance.Application.Dtos.Base;

namespace Finance.Application.Dtos.CreditCards;

public record CreditCardStatementImportTemplateDto : Dto<Guid>
{
    public CreditCardStatementImportTemplateDto() { }

    public string Name { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
    public Guid? UserId { get; set; }
    public string ConfigJson { get; set; } = string.Empty;
}
