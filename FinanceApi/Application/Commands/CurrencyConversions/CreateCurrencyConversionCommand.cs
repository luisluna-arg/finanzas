using FinanceApi.Domain.Models;
using MediatR;

namespace FinanceApi.Application.Commands.CurrencyConversions;

public class CreateCurrencyConversionCommand : IRequest<CurrencyConversion>
{
    required public Guid MovementId { get; set; }

    public Guid? CurrencyId { get; set; }

    public decimal Amount { get; set; }
}
