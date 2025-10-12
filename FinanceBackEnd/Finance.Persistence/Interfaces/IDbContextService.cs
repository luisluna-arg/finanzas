namespace Finance.Persistence.Interfaces;

public interface IDbContextService : IDisposable
{
    FinanceDbContext GetDbContext();
}