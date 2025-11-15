using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Incomes;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Incomes;

public class GetIncomesQueryHandler : BaseCollectionHandler<GetIncomesQuery, Income>
{
    private readonly IRepository<Income, Guid> repository;

    public GetIncomesQueryHandler(
        FinanceDbContext db,
        IRepository<Income, Guid> movementRepository)
        : base(db)
    {
        this.repository = movementRepository;
    }

    public override async Task<DataResult<List<Income>>> ExecuteAsync(GetIncomesQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Income> query = repository.GetDbSet()
            .Include(o => o.Currency)
            .Include(o => o.Bank)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        if (request.From.HasValue)
        {
            query = query.FilterBy("TimeStamp", Application.Repositories.Base.ExpressionOperator.GreaterThanOrEqual, request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.FilterBy("TimeStamp", Application.Repositories.Base.ExpressionOperator.LessThanOrEqual, request.To.Value);
        }

        if (request.CurrencyId.HasValue)
        {
            query = query.Where(o => o.CurrencyId == request.CurrencyId.Value);
        }

        if (request.BankId.HasValue)
        {
            query = query.Where(o => o.BankId == request.BankId.Value);
        }

        return DataResult<List<Income>>.Success(await query.ToListAsync(cancellationToken));
    }
}

public class GetIncomesQuery : GetAllQuery<Income>
{
    /// <summary>
    /// Gets or sets date to filter from. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? From { get; set; }
    /// <summary>
    /// Gets or sets date to filter to. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? To { get; set; }
    public Guid? CurrencyId { get; set; }
    public Guid? BankId { get; set; }
}
