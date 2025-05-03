using Finance.Application.Dtos.Banks;

namespace Finance.Application.Dtos.CreditCards;

public record CreditCardDto : Dto<Guid>
{
    public CreditCardDto()
        : base()
    {
    }

    public Guid BankId { get; set; }

    public BankDto Bank { get; set; } = default!;

    public CreditCardStatementDto CreditCardStatement { get; set; } = default!;

    public int RecordCount { get; set; }

    required public string Name { get; set; } = string.Empty;
}
