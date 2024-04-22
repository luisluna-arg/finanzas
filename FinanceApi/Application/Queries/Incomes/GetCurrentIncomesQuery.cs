using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.Incomes;

public class GetCurrentIncomesQueryHandler : BaseResponseHandler<GetCurrentIncomesQuery, ICollection<Income>>
{
    private readonly IRepository<Income, Guid> movementRepository;

    public GetCurrentIncomesQueryHandler(
        FinanceDbContext db,
        IRepository<Income, Guid> movementRepository)
        : base(db)
    {
        this.movementRepository = movementRepository;
    }

    public override async Task<ICollection<Income>> Handle(GetCurrentIncomesQuery request, CancellationToken cancellationToken)
    {
        var query = movementRepository.GetDbSet()
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

public class GetCurrentIncomesQuery(
    Guid? bankId = default,
    Guid? currencyId = default)
    : IRequest<ICollection<Income>>
{
    public Guid? BankId { get => bankId; }
    public Guid? CurrencyId { get => currencyId; }
}
