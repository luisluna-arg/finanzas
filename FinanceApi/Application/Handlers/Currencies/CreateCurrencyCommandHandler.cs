using FinanceApi.Application.Commands.Currencies;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;

namespace FinanceApi.Application.Handlers.Currencies;

public class CreateCurrencyCommandHandler : BaseResponseHandler<CreateCurrencyCommand, Currency>
{
    public CreateCurrencyCommandHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<Currency> Handle(CreateCurrencyCommand command, CancellationToken cancellationToken)
    {
        var newCurrency = new Currency()
        {
            ShortName = command.ShortName,
            Name = command.Name
        };

        DbContext.Currency.Add(newCurrency);
        await DbContext.SaveChangesAsync();

        return await Task.FromResult(newCurrency);
    }
}
