using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories.Base;

namespace FinanceApi.Infrastructure.Repositotories;

public class CurrencyConversionRepository : BaseRepository<CurrencyConversion, Guid>
{
    public CurrencyConversionRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
