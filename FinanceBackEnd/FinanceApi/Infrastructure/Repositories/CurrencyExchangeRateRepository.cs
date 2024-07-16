using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;

namespace FinanceApi.Infrastructure.Repositories;

public class CurrencyExchangeRateRepository : BaseRepository<CurrencyExchangeRate, Guid>
{
    public CurrencyExchangeRateRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
