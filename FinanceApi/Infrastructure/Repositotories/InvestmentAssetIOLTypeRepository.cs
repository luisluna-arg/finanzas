using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories.Base;

namespace FinanceApi.Infrastructure.Repositotories;

public class InvestmentAssetIOLTypeRepository : BaseRepository<InvestmentAssetIOLType, Guid>
{
    public InvestmentAssetIOLTypeRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
