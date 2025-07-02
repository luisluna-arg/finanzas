using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class IOLInvestmentRepository(FinanceDbContext dbContext) : BaseRepository<IOLInvestment, Guid>(dbContext);
