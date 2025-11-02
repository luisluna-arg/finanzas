using Finance.Domain.SpecialTypes;
using CQRSDispatch.Interfaces;

namespace Finance.Application.Commands.Movements;

public abstract class CreateMovementBaseCommand : ICommand
{
    public Guid? CurrencyId { get; set; } = null;
    required public DateTime TimeStamp { get; set; }
    required public string Concept1 { get; set; }
    required public string? Concept2 { get; set; }
    required public Money Amount { get; set; }
    required public Money? Total { get; set; }
}
