using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class CurrencySymbolRepository(FinanceDbContext dbContext) : BaseRepository<CurrencySymbol, Guid>(dbContext);
