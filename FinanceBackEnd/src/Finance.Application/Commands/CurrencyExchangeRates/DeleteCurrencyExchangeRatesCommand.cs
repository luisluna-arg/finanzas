using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Services;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Commands.CurrencyExchangeRates;

public record DeleteCurrencyExchangeRatesCommand(Guid[] Ids) : ICommand<CommandResult>;

public class DeleteCurrencyExchangeRatesCommandHandler(IEntityService<CurrencyExchangeRate, Guid> service)
    : ICommandHandler<DeleteCurrencyExchangeRatesCommand>
{
    public async Task<CommandResult> ExecuteAsync(DeleteCurrencyExchangeRatesCommand request, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(request.Ids, cancellationToken);
        return CommandResult.Success();
    }
}
