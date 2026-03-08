using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Application.Repositories;
using Finance.Application.Repositories.Base;
using Finance.Domain.Models.Incomes;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Incomes;

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

public class GetIncomesQueryHandler : BaseCollectionQueryHandler<GetIncomesQuery, Income>
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
            query = query.FilterBy("TimeStamp", ExpressionOperator.GreaterThanOrEqual, request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.FilterBy("TimeStamp", ExpressionOperator.LessThanOrEqual, request.To.Value);
        }

        if (request.CurrencyId.HasValue)
        {
            query = query.Where(o => o.CurrencyId == request.CurrencyId.Value);
        }

        if (request.BankId.HasValue)
        {
            query = query.Where(o => o.BankId == request.BankId.Value);
        }

        var result = await query
            .OrderByDescending(o => o.TimeStamp)
            .ToListAsync(cancellationToken);

        return DataResult<List<Income>>.Success(result);
    }
}
