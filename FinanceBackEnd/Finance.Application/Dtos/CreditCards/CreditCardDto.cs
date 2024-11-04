using Finance.Application.Dtos.Banks;

namespace Finance.Application.Dtos.CreditCards;

public record CreditCardDto : Dto<Guid>
{
    public CreditCardDto()
        : base()
    {
    }

    public Guid BankId { get; set; }

    public BankDto Bank { get; set; }

    public CreditCardStatementDto? CreditCardStatement { get; set; }

    public int RecordCount { get; set; }

    required public string Name { get; set; }
}
