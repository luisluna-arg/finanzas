using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Movements;

public class GetFundMovementsQueryHandler : BaseCollectionHandler<GetFundMovementsQuery, Movement?>
{
    private readonly IAppModuleRepository appModuleRepository;
    private readonly IRepository<Movement, Guid> repository;

    public GetFundMovementsQueryHandler(
        FinanceDbContext db,
        IAppModuleRepository appModuleRepository,
        IRepository<Movement, Guid> movementRepository)
        : base(db)
    {
        this.appModuleRepository = appModuleRepository;
        this.repository = movementRepository;
    }

    public override async Task<ICollection<Movement?>> Handle(GetFundMovementsQuery request, CancellationToken cancellationToken)
    {
        var fundModule = await appModuleRepository.GetFundsAsync(cancellationToken);
        if (fundModule == null) throw new Exception($"Funds module not found");

        IQueryable<Movement> query = repository.GetDbSet()
            .Include(o => o.AppModule)
            .Include(o => o.Currency)
            .Include(o => o.Bank)
            .Where(o => o.AppModuleId == fundModule.Id)
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

        return await query.ToArrayAsync();
    }
}

public class GetFundMovementsQuery : GetAllQuery<Movement?>
{
    /// <summary>
    /// Gets or sets date to filter from. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? From { get; set; }

    /// <summary>
    /// Gets or sets date to filter to. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? To { get; set; }
}
