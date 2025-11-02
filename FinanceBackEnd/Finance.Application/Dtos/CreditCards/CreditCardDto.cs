using Finance.Application.Dtos.Base;
using Finance.Application.Dtos.Banks;

namespace Finance.Application.Dtos.CreditCards;

public record CreditCardDto : Dto<Guid>
{
    public CreditCardDto() { }

    public Guid BankId { get; set; } = Guid.Empty;
    public BankDto Bank { get; set; } = default!;
    public CreditCardStatementDto CreditCardStatement { get; set; } = default!;
    public int RecordCount { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
}
