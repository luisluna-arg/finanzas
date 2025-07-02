using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class CreditCardStatementRepository(FinanceDbContext dbContext) : BaseRepository<CreditCardStatement, Guid>(dbContext);
