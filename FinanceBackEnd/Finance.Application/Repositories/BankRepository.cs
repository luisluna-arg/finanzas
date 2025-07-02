using Finance.Domain.Models;
using Finance.Persistance;
using Finance.Application.Repositories.Base;

namespace Finance.Application.Repositories;

public class BankRepository(FinanceDbContext dbContext) : BaseRepository<Bank, Guid>(dbContext);
