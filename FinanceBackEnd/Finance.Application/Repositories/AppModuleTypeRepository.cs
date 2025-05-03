using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class AppModuleTypeRepository : BaseRepository<AppModuleType, short>
{
    public AppModuleTypeRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
