using Finance.Domain.Models;

namespace Finance.Application.Repositories;

public interface IAppModuleRepository : IRepository<AppModule, Guid>
{
    Task<AppModule> GetFundsAsync(CancellationToken cancellationToken);

    Task<AppModule> GetDollarFundsAsync(CancellationToken cancellationToken);

    new Task<AppModule> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
