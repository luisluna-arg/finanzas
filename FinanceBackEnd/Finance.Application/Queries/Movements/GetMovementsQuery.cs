using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Application.Repositories;
using Finance.Application.Repositories.Base;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Movements;

public class GetMovementsQueryHandler : BaseCollectionHandler<GetMovementsQuery, Movement>
{
    private readonly IRepository<Movement, Guid> movementRepository;

    public GetMovementsQueryHandler(
        FinanceDbContext db,
        IRepository<Movement, Guid> movementRepository)
        : base(db)
    {
        this.movementRepository = movementRepository;
    }

    public override async Task<DataResult<List<Movement>>> ExecuteAsync(GetMovementsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Movement> query = movementRepository.GetDbSet()
            .Include(o => o.AppModule)
            .Include(o => o.Currency)
            .Include(o => o.Bank).AsQueryable();

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

        if (request.AppModuleId.HasValue)
        {
            query = query.Where(o => o.AppModuleId == request.AppModuleId.Value);
        }

        if (request.BankId.HasValue)
        {
            query = query.Where(o => o.BankId == request.BankId.Value);
        }

        return DataResult<List<Movement>>.Success(await query.ToListAsync(cancellationToken));
    }
}

public class GetMovementsQuery : GetAllQuery<Movement>
{
    public Guid? AppModuleId { get; private set; }

    public Guid? BankId { get; private set; }

    /// <summary>
    /// Gets or sets date to filter from. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? From { get; set; }

    /// <summary>
    /// Gets or sets date to filter to. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? To { get; set; }
}
