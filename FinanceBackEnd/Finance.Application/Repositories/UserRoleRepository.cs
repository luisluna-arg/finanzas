using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class UserRoleRepository(FinanceDbContext dbContext) : BaseRepository<UserRole, Guid>(dbContext);
