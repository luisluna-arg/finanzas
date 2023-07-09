using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories.Base;

namespace FinanceApi.Infrastructure.Repositotories;

public class InvestmentAssetIOLRepository : BaseRepository<InvestmentAssetIOL, Guid>
{
    public InvestmentAssetIOLRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
