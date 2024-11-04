using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.CurrencyConversions;

public class ActivateCurrencyConversionCommandHandler : IRequestHandler<ActivateCurrencyConversionCommand, CurrencyConversion?>
{
    private readonly IEntityService<CurrencyConversion, Guid> service;

    public ActivateCurrencyConversionCommandHandler(
        IEntityService<CurrencyConversion, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<CurrencyConversion?> Handle(ActivateCurrencyConversionCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivatedAsync(request.Id, false, cancellationToken);
}

public class ActivateCurrencyConversionCommand : IRequest<CurrencyConversion?>
{
    public Guid Id { get; set; }
}
