using Finance.Application.Base.Handlers;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;

namespace Finance.Application.Commands.CurrencyExchangeRates;

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
            await currencyRepository.DeleteAsync(id, cancellationToken);
        }
    }
}

public class DeleteCurrencyExchangeRateCommand : IRequest
{
    public Guid[] Ids { get; set; } = [];
}
