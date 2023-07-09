using MediatR;

namespace FinanceApi.Application.Commands.CurrencyConversions;

public class DeleteCurrencyConversionCommand : IRequest
{
    required public Guid Id { get; set; }
}
