namespace Finance.Persistance.Interfaces;

public interface IDbContextService : IDisposable
{
    FinanceDbContext GetDbContext();
}