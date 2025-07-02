using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class RoleRepository(FinanceDbContext dbContext) : BaseRepository<Role, RoleEnum>(dbContext);
