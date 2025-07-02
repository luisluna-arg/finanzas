using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class IOLInvestmentAssetRepository(FinanceDbContext dbContext) : BaseRepository<IOLInvestmentAsset, Guid>(dbContext);
