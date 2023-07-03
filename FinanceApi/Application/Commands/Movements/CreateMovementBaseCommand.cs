using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain.Models;
using MediatR;

namespace FinanceApi.Application.Commands.Movements;

public abstract class CreateMovementBaseCommand : IRequest<Movement>
{
    public Guid? CurrencyId { get; set; } = null;

    required public DateTime TimeStamp { get; set; }

    required public string Concept1 { get; set; }

    required public string? Concept2 { get; set; }

    required public Money Amount { get; set; }

    required public Money? Total { get; set; }
}
