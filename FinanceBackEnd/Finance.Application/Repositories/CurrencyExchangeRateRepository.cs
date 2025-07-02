using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class CurrencyExchangeRateRepository(FinanceDbContext dbContext) : BaseRepository<CurrencyExchangeRate, Guid>(dbContext);
