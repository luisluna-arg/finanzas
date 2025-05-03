using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class IOLInvestmentAssetRepository : BaseRepository<IOLInvestmentAsset, Guid>
{
    public IOLInvestmentAssetRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
