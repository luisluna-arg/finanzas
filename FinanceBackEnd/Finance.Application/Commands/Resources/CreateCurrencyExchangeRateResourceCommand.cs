using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands;

public class CreateCurrencyExchangeRatePermissionsCommand : CreateResourcePermissionsCommand<CurrencyExchangeRate, Guid, CurrencyExchangeRatePermissions>
{
}

public class CreateCurrencyExchangeRatePermissionsCommandHandler(FinanceDbContext dbContext)
    : CreateResourcePermissionsCommandHandler<CreateCurrencyExchangeRatePermissionsCommand, CurrencyExchangeRate, Guid, CurrencyExchangeRatePermissions>(dbContext)
{
    protected override async Task<CurrencyExchangeRate?> QuerySource(CreateCurrencyExchangeRatePermissionsCommand request, CancellationToken cancellationToken)
    {
        return await DbContext.CurrencyExchangeRate
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == request.ResourceId, cancellationToken);
    }
}
