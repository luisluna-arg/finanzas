using FinanceApi.Application.Commands.Currencies;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;

namespace FinanceApi.Application.Handlers.Currencies;

public class CreateCurrencyCommandHandler : IRequestHandler<CreateCurrencyCommand, Currency>
{
    private readonly FinanceDbContext dbContext;

    public CreateCurrencyCommandHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<Currency> Handle(CreateCurrencyCommand command, CancellationToken cancellationToken)
    {
        var newCurrency = new Currency()
        {
            ShortName = command.ShortName,
            Name = command.Name
        };

        dbContext.Currency.Add(newCurrency);
        await dbContext.SaveChangesAsync();

        return await Task.FromResult(newCurrency);
    }
}
