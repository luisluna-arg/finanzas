using Finance.Application.Base.Handlers;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistence;

namespace Finance.Application.Commands.CurrencyConvertions;

public class DeleteCurrencyConversionCommandHandler : BaseResponselessHandler<DeleteCurrencyConversionCommand>
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

public class DeleteCurrencyConversionCommand : ICommand
{
    required public Guid Id { get; set; }
}
