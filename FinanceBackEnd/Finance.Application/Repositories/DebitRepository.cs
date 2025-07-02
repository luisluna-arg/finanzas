using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class DebitRepository(FinanceDbContext dbContext) : BaseRepository<Debit, Guid>(dbContext);
