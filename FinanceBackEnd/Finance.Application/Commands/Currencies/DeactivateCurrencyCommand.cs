using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.Currencies;

public class DeactivateCurrencyCommandHandler : IRequestHandler<DeactivateCurrencyCommand, Currency?>
{
    private readonly IEntityService<Currency, Guid> service;

    public DeactivateCurrencyCommandHandler(
        IEntityService<Currency, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<Currency?> Handle(DeactivateCurrencyCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivatedAsync(request.Id, true, cancellationToken);
}

public class DeactivateCurrencyCommand : IRequest<Currency?>
{
    public Guid Id { get; set; }
}
