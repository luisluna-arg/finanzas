using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Repositories;

public class CurrencyRepository(FinanceDbContext dbContext) : BaseRepository<Currency, Guid>(dbContext)
{
    public override async Task<Currency?> GetByAsync(string searchCriteria, object searchValue, CancellationToken cancellationToken)
    {
        if (searchCriteria == "Symbol")
        {
            var symbols = searchValue as IEnumerable<string>;

            if (symbols?.Count() > 0)
            {
                return await DbContext.Currency
                    .Include(c => c.Symbols)
                    .Where(c => c.Symbols.Any(s => symbols.Contains(s.Symbol)))
                    .FirstAsync(c => c.Symbols.Any(s => symbols.Contains(s.Symbol)), cancellationToken);
            }

            return null;
        }

        return await base.GetByAsync(searchCriteria, searchValue, cancellationToken);
    }
}
