using FinanceApi.Application.Commands.Currencies;
using FinanceApi.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Currencies;

public class CreateCurrencyCommandHandler : IRequestHandler<CreateCurrencyCommand, Currency>
{
    private readonly FinanceDbContext dbContext;

    public CreateCurrencyCommandHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<Currency> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var newCurrency = new Currency()
        {
            ShortName = request.ShortName,
            Name = request.Name
        };

        dbContext.Currency.Add(newCurrency);
        await dbContext.SaveChangesAsync();

        return await Task.FromResult(newCurrency);
    }
}
