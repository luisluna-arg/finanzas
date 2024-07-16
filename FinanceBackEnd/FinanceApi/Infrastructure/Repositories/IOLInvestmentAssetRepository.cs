using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;

namespace FinanceApi.Infrastructure.Repositories;

public class IOLInvestmentAssetRepository : BaseRepository<IOLInvestmentAsset, Guid>
{
    public IOLInvestmentAssetRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
