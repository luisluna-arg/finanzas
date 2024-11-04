using Finance.Domain;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class FrequencyRepository : BaseRepository<Frequency, FrequencyEnum>
{
    public FrequencyRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
