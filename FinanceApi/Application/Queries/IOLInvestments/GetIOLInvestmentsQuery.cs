using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.IOLInvestments;

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

    public override async Task<ICollection<IOLInvestment>> Handle(GetIOLInvestmentsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<IOLInvestment> query = repository.GetDbSet().Include(o => o.Asset).ThenInclude(o => o.Type).AsQueryable();

        if (request.From.HasValue)
        {
            query = query.FilterBy("TimeStamp", Infrastructure.Repositories.Base.ExpressionOperator.GreaterThanOrEqual, request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.FilterBy("TimeStamp", Infrastructure.Repositories.Base.ExpressionOperator.LessThanOrEqual, request.To.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.AssetSymbol))
        {
            query = query.Where(o => o.Asset.Symbol == request.AssetSymbol!);
        }

        return await query.ToArrayAsync();
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
