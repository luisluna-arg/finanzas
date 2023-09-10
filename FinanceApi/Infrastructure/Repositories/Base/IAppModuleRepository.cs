using FinanceApi.Domain.Models;

namespace FinanceApi.Infrastructure.Repositories;

public interface IAppModuleRepository : IRepository<AppModule, Guid>
{
    Task<AppModule> GetFunds();
}
