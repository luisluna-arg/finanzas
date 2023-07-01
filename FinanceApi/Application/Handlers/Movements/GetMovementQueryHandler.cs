using FinanceApi.Application.Queries.Movements;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Movements;

public class GetMovementQueryHandler : BaseResponseHandler<GetMovementQuery, Movement>
{
    public GetMovementQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<Movement> Handle(GetMovementQuery request, CancellationToken cancellationToken)
    {
        var movement = await DbContext.Movement.FirstOrDefaultAsync(o => o.Id == request.Id);
        if (movement == null) throw new Exception("Movement not found");

        return await Task.FromResult(movement);
    }
}
