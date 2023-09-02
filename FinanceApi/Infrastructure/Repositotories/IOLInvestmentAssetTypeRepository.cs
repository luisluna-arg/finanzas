using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories.Base;

namespace FinanceApi.Infrastructure.Repositotories;

public class IOLInvestmentAssetTypeRepository : BaseRepository<IOLInvestmentAssetType, ushort>
{
    public IOLInvestmentAssetTypeRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
