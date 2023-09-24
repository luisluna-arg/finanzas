using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.CurrencyConversions;

public class DeactivateCurrencyConversionCommandHandler : IRequestHandler<DeactivateCurrencyConversionCommand, CurrencyConversion?>
{
    private readonly IEntityService<CurrencyConversion, Guid> service;

    public DeactivateCurrencyConversionCommandHandler(
        IEntityService<CurrencyConversion, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<CurrencyConversion?> Handle(DeactivateCurrencyConversionCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivated(request.Id, true);
}

public class DeactivateCurrencyConversionCommand : IRequest<CurrencyConversion?>
{
    public Guid Id { get; set; }
}
