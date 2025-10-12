using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;
using Finance.Application.Repositories.Base;

namespace Finance.Application.Queries.Funds;

public class GetFundsQueryHandler : BaseCollectionHandler<GetFundsQuery, Fund>
{
    private readonly IRepository<Fund, Guid> repository;

    public GetFundsQueryHandler(
        FinanceDbContext db,
        IRepository<Fund, Guid> movementRepository)
        : base(db)
    {
        this.repository = movementRepository;
    }

    public override async Task<DataResult<List<Fund>>> ExecuteAsync(GetFundsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Fund> query = repository.GetDbSet()
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

        if (request.DailyUse.HasValue)
        {
            query = query.Where(o => o.DailyUse == request.DailyUse.Value);
        }

        return DataResult<List<Fund>>.Success(await query.ToListAsync(cancellationToken));
    }
}

public class GetFundsQuery : GetAllQuery<Fund>
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

    public bool? DailyUse { get; set; }
}
