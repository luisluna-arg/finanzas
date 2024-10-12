using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;

namespace FinanceApi.Infrastructure.Repositories;

public class FrequencyRepository : BaseRepository<Frequency, FrequencyEnum>
{
    public FrequencyRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
