using Finance.Domain.Models;
using Finance.Domain.Enums;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class AppModuleTypeRepository(FinanceDbContext dbContext) : BaseRepository<AppModuleType, AppModuleTypeEnum>(dbContext);
