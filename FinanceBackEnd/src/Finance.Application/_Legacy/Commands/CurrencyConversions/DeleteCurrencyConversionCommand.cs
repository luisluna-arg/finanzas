using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;

namespace Finance.Application.Legacy.Commands.CurrencyConvertions;

public sealed class DeleteCurrencyConversionCommandHandler : BaseResponselessHandler<DeleteCurrencyConversionCommand>
{
    private readonly IRepository<CurrencyConversion, Guid> currencyConversionRepository;

    public DeleteCurrencyConversionCommandHandler(
        FinanceDbContext db,
        IRepository<CurrencyConversion, Guid> currencyConversionRepository)
        : base(db)
    {
        this.currencyConversionRepository = currencyConversionRepository;
    }

    public override async Task<CommandResult> ExecuteAsync(DeleteCurrencyConversionCommand command, CancellationToken cancellationToken)
    {
        await currencyConversionRepository.DeleteAsync(command.Id, cancellationToken);
        return CommandResult.Success();
    }
}

public sealed class DeleteCurrencyConversionCommand : ICommand
{
    required public Guid Id { get; set; }
}
