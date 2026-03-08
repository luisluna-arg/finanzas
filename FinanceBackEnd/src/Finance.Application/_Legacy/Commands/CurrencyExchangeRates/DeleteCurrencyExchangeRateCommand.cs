using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;

namespace Finance.Application.Legacy.Commands.CurrencyExchangeRates;

public class DeleteCurrencyExchangeRateCommand : ICommand<DataResult<bool>>
{
    public Guid[] Ids { get; set; } = [];
}

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

    public override async Task<CommandResult> ExecuteAsync(DeleteCurrencyExchangeRateCommand command, CancellationToken cancellationToken)
    {
        foreach (Guid id in command.Ids)
        {
            await currencyRepository.DeleteAsync(id, cancellationToken);
        }

        return CommandResult.Success();
    }
}
