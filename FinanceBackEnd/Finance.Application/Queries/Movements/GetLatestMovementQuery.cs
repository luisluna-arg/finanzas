using Finance.Application.Base.Handlers;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Movements;

public class GetLatestMovementQueryHandler : BaseResponseHandler<GetLatestMovementQuery, Movement?>
{
    private readonly IRepository<Movement, Guid> movementRepository;

    public GetLatestMovementQueryHandler(
        FinanceDbContext db,
        IRepository<Movement, Guid> movementRepository)
        : base(db)
    {
        this.movementRepository = movementRepository;
    }

    public override async Task<Movement?> Handle(GetLatestMovementQuery request, CancellationToken cancellationToken)
        => await movementRepository.GetDbSet().Include(o => o.AppModule).FirstOrDefaultAsync(o => o.AppModuleId == request.AppModuleId);
}

public class GetLatestMovementQuery : IRequest<Movement>
{
    private Guid appModuleId;

    public GetLatestMovementQuery(Guid appModuleId)
    {
        this.appModuleId = appModuleId;
    }

    public Guid AppModuleId { get => appModuleId; }
}
