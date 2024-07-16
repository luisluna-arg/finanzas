using FinanceApi.Domain.Models;

namespace FinanceApi.Infrastructure.Repositories;

public interface IAppModuleRepository : IRepository<AppModule, Guid>
{
    Task<AppModule> GetFundsAsync(CancellationToken cancellationToken);

    Task<AppModule> GetDollarFundsAsync(CancellationToken cancellationToken);
}
