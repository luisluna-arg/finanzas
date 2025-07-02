using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class IOLInvestmentAssetTypeRepository(FinanceDbContext dbContext) : BaseRepository<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum>(dbContext);
