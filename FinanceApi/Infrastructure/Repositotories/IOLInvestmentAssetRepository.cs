using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories.Base;

namespace FinanceApi.Infrastructure.Repositotories;

public class IOLInvestmentAssetRepository : BaseRepository<IOLInvestmentAsset, Guid>
{
    public IOLInvestmentAssetRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
