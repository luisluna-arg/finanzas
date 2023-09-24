using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Services;
using MediatR;

namespace FinanceApi.Application.Commands.Currencys;

public class ActivateCurrencyCommandHandler : IRequestHandler<ActivateCurrencyCommand, Currency?>
{
    private readonly IEntityService<Currency, Guid> service;

    public ActivateCurrencyCommandHandler(
        IEntityService<Currency, Guid> repository)
    {
        this.service = repository;
    }

    public async Task<Currency?> Handle(ActivateCurrencyCommand request, CancellationToken cancellationToken)
        => await service.SetDeactivated(request.Id, false);
}

public class ActivateCurrencyCommand : IRequest<Currency?>
{
    public Guid Id { get; set; }
}
