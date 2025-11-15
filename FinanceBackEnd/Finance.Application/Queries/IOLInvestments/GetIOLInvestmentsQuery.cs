using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Application.Repositories;
using Finance.Application.Repositories.Base;
using Finance.Domain.Models.IOLInvestments;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.IOLInvestments;

public class GetAllIOLInvestmentQueryHandler : BaseCollectionHandler<GetIOLInvestmentsQuery, IOLInvestment>
{
    private readonly IRepository<IOLInvestment, Guid> repository;

    public GetAllIOLInvestmentQueryHandler(
        FinanceDbContext db,
        IRepository<IOLInvestment, Guid> investmentAssetIOLRepository)
        : base(db)
    {
        repository = investmentAssetIOLRepository;
    }

    public override async Task<DataResult<List<IOLInvestment>>> ExecuteAsync(GetIOLInvestmentsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<IOLInvestment> query = repository.GetDbSet().Include(o => o.Asset).ThenInclude(o => o.Type).AsQueryable();

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

        if (!string.IsNullOrWhiteSpace(request.AssetSymbol))
        {
            query = query.Where(o => o.Asset.Symbol == request.AssetSymbol!);
        }

        return DataResult<List<IOLInvestment>>.Success(await query.ToListAsync(cancellationToken));
    }
}

public class GetIOLInvestmentsQuery : GetAllQuery<IOLInvestment>
{
    /// <summary>
    /// Gets or sets date to filter from. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? From { get; set; }
    /// <summary>
    /// Gets or sets date to filter to. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? To { get; set; }
    public string? AssetSymbol { get; set; }
}
