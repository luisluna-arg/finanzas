using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Movements;

public class GetLatestMovementQueryHandler : BaseQueryHandler<GetLatestMovementQuery, Movement?>
{
    private readonly IRepository<Movement, Guid> movementRepository;

    public GetLatestMovementQueryHandler(
        FinanceDbContext db,
        IRepository<Movement, Guid> movementRepository)
        : base(db)
    {
        this.movementRepository = movementRepository;
    }

    public override async Task<DataResult<Movement?>> ExecuteAsync(GetLatestMovementQuery request, CancellationToken cancellationToken)
    {
        var result = await movementRepository.GetDbSet().Include(o => o.AppModule).FirstOrDefaultAsync(o => o.AppModuleId == request.AppModuleId);
        return DataResult<Movement?>.Success(result);
    }
}

public class GetLatestMovementQuery : IQuery<Movement?>
{
    private Guid appModuleId;

    public GetLatestMovementQuery(Guid appModuleId)
    {
        this.appModuleId = appModuleId;
    }

    public Guid AppModuleId { get => appModuleId; }
}
