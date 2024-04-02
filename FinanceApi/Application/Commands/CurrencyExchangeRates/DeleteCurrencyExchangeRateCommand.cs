using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.CurrencyExchangeRates;

public class DeleteCurrencyExchangeRateCommandHandler : BaseResponselessHandler<DeleteCurrencyExchangeRateCommand>
{
    private readonly IRepository<CurrencyExchangeRate, Guid> currencyRepository;

    public DeleteCurrencyExchangeRateCommandHandler(
        FinanceDbContext db,
        IRepository<CurrencyExchangeRate, Guid> currencyRepository)
        : base(db)
    {
        this.currencyRepository = currencyRepository;
    }

    public override async Task Handle(DeleteCurrencyExchangeRateCommand command, CancellationToken cancellationToken)
    {
        foreach (Guid id in command.Ids)
        {
            await currencyRepository.Delete(id);
        }
    }
}

public class DeleteCurrencyExchangeRateCommand : IRequest
{
    public Guid[] Ids { get; set; }
}
