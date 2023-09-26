using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;

namespace FinanceApi.Infrastructure.Repositories;

public class CreditCardIssuerRepository : BaseRepository<CreditCardIssuer, Guid>
{
    public CreditCardIssuerRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
