using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApiApplication.Queries.Summary;

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

        query = query
            .GroupBy(g => new { g.BankId, g.CurrencyId })
            .Select(g => g.OrderByDescending(i => i.TimeStamp).First())
            .AsQueryable();

        return await query.ToArrayAsync();
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
