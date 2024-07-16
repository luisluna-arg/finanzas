using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.Movements;

public class GetMovementsQueryHandler : BaseCollectionHandler<GetMovementsQuery, Movement?>
{
    private readonly IRepository<Movement, Guid> movementRepository;

    public GetMovementsQueryHandler(
        FinanceDbContext db,
        IRepository<Movement, Guid> movementRepository)
        : base(db)
    {
        this.movementRepository = movementRepository;
    }

    public override async Task<ICollection<Movement?>> Handle(GetMovementsQuery request, CancellationToken cancellationToken)
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
            query = query.FilterBy("TimeStamp", Infrastructure.Repositories.Base.ExpressionOperator.GreaterThanOrEqual, request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.FilterBy("TimeStamp", Infrastructure.Repositories.Base.ExpressionOperator.LessThanOrEqual, request.To.Value);
        }

        if (request.AppModuleId.HasValue)
        {
            query = query.Where(o => o.AppModuleId == request.AppModuleId.Value);
        }

        if (request.BankId.HasValue)
        {
            query = query.Where(o => o.BankId == request.BankId.Value);
        }

        return await query.ToArrayAsync();
    }
}

public class GetMovementsQuery : GetAllQuery<Movement?>
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
