using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;

namespace FinanceApi.Infrastructure.Repositories;

public class AppModuleTypeRepository : BaseRepository<AppModuleType, short>
{
    public AppModuleTypeRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
