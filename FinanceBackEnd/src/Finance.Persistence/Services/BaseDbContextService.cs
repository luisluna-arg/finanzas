using Finance.Persistence.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.Persistence.Services;

public class BaseDbContextService : IDbContextService
{
    protected readonly IServiceProvider _serviceProvider;
    protected IServiceScope? _scope;
    protected FinanceDbContext? _dbContext;

    public BaseDbContextService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public FinanceDbContext GetDbContext()
    {
        if (_dbContext == null)
        {
            CreateDbContext();
        }
        return _dbContext!;
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
        _scope?.Dispose();
    }

    protected void CreateDbContext()
    {
        _scope = _serviceProvider.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
    }
}
