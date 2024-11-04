using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.CurrencyConversions;

public class DeactivateCurrencyConversionCommandHandler : IRequestHandler<DeactivateCurrencyConversionCommand, CurrencyConversion?>
{
    private readonly IEntityService<CurrencyConversion, Guid> service;

    public DeactivateCurrencyConversionCommandHandler(
        IEntityService<CurrencyConversion, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<CurrencyConversion?> Handle(DeactivateCurrencyConversionCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivatedAsync(request.Id, true, cancellationToken);
}

public class DeactivateCurrencyConversionCommand : IRequest<CurrencyConversion?>
{
    public Guid Id { get; set; }
}
