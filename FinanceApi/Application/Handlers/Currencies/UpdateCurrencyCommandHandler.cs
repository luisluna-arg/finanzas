using FinanceApi.Application.Commands.Currencies;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.Currencies;

public class UpdateCurrencyCommandHandler : BaseResponseHandler<UpdateCurrencyCommand, Currency>
{
    private readonly IRepository<Currency, Guid> _currencyRepository;

    public UpdateCurrencyCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository)
        : base(db)
    {
        _currencyRepository = currencyRepository;
    }

    public override async Task<Currency> Handle(UpdateCurrencyCommand command, CancellationToken cancellationToken)
    {
        var currency = await _currencyRepository.GetById(command.Id);

        currency.Name = command.Name;
        currency.ShortName = command.ShortName;

        await DbContext.SaveChangesAsync();

        return await Task.FromResult(currency);
    }
}
