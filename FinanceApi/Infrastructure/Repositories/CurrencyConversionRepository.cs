using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;

namespace FinanceApi.Infrastructure.Repositories;

public class CurrencyConversionRepository : BaseRepository<CurrencyConversion, Guid>
{
    public CurrencyConversionRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
