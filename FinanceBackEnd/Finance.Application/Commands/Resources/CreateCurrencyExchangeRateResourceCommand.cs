using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands;

public class CreateCurrencyExchangeRateResourceCommand : CreateEntityResourceCommand<CurrencyExchangeRate, Guid, CurrencyExchangeRateResource>
{
    public Guid Id { get; set; }
}

public class CreateCurrencyExchangeRateResourceCommandHandler(FinanceDbContext dbContext)
    : CreateEntityResourceCommandHandler<CreateCurrencyExchangeRateResourceCommand, CurrencyExchangeRate, Guid, CurrencyExchangeRateResource>(dbContext)
{
    protected override async Task<CurrencyExchangeRate?> QuerySource(CreateCurrencyExchangeRateResourceCommand request, CancellationToken cancellationToken)
    {
        return await DbContext.CurrencyExchangeRate
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
    }
}
