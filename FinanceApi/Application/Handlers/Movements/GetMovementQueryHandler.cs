using FinanceApi.Application.Queries.Movements;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Movements;

public class GetMovementQueryHandler : IRequestHandler<GetMovementQuery, Movement>
{
    private readonly FinanceDbContext dbContext;

    public GetMovementQueryHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<Movement> Handle(GetMovementQuery request, CancellationToken cancellationToken)
    {
        var movement = await dbContext.Movement.FirstOrDefaultAsync(o => o.Id == request.Id);
        if (movement == null) throw new Exception("Movement not found");

        return await Task.FromResult(movement);
    }
}
