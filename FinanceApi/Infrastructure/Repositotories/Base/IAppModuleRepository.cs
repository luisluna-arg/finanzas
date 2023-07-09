using FinanceApi.Domain.Models;

namespace FinanceApi.Infrastructure.Repositotories;

public interface IAppModuleRepository : IRepository<AppModule, Guid>
{
    Task<AppModule> GetFund();
}
