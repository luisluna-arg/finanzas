using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;

namespace FinanceApi.Infrastructure.Repositories;

public class IOLInvestmentAssetTypeRepository : BaseRepository<IOLInvestmentAssetType, ushort>
{
    public IOLInvestmentAssetTypeRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
