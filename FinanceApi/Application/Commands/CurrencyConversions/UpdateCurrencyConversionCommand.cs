using FinanceApi.Domain.Models;
using MediatR;

namespace FinanceApi.Application.Commands.CurrencyConversions;

public class UpdateCurrencyConversionCommand : IRequest<CurrencyConversion>
{
    required public Guid Id { get; set; }

    required public Guid MovementId { get; set; }

    public Guid? CurrencyId { get; set; }

    public decimal Amount { get; set; }
}
