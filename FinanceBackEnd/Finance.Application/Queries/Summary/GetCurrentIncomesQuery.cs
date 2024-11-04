using Finance.Application.Base.Handlers;
using Finance.Application.Extensions;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Summary;

public class GetCurrentIncomesQueryHandler : BaseResponseHandler<GetCurrentIncomesQuery, ICollection<Income>>
{
    private readonly IRepository<Income, Guid> incomeRepository;

    public GetCurrentIncomesQueryHandler(
        FinanceDbContext db,
        IRepository<Income, Guid> incomeRepository)
        : base(db)
    {
        this.incomeRepository = incomeRepository;
    }

    public override async Task<ICollection<Income>> Handle(GetCurrentIncomesQuery request, CancellationToken cancellationToken)
    {
        var query = incomeRepository.GetDbSet()
            .Include(o => o.Bank)
            .Include(o => o.Currency)
            .AsQueryable();

        if (request.BankId.HasValue)
        {
            query = query.Where(q => q.BankId == request.BankId.Value);
        }

        if (request.CurrencyId.HasValue)
        {
            query = query.Where(q => q.CurrencyId == request.CurrencyId.Value);
        }

        var dateFilter = DateTime.UtcNow.CurrentMonth().AddMonths(-2);

        var data = await query.Where(q => q.TimeStamp >= dateFilter).ToArrayAsync();

        data = data.GroupBy(g => new { g.BankId, g.CurrencyId })
            .SelectMany(g =>
            {
                var current = g.OrderByDescending(i => i.TimeStamp).ThenBy(i => i.Id).First();
                var refDate = current.TimeStamp.CurrentMonth();
                return g.Where(o => o.TimeStamp >= refDate).OrderByDescending(i => i.TimeStamp).ThenBy(i => i.Id).ToArray();
            })
            .ToArray();

        return data;
    }
}

public class GetCurrentIncomesQuery : IRequest<ICollection<Income>>
{
    public GetCurrentIncomesQuery(
        Guid? bankId = default,
        Guid? currencyId = default)
    {
        BankId = bankId;
        CurrencyId = currencyId;
    }

    public Guid? BankId { get; private set; }
    public Guid? CurrencyId { get; private set; }
}
