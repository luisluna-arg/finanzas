using Finance.Application.Repositories.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;

namespace Finance.Application.Repositories;

public class CreditCardStatementImportTemplateRepository(FinanceDbContext dbContext)
    : BaseRepository<CreditCardStatementImportTemplate, Guid>(dbContext);
