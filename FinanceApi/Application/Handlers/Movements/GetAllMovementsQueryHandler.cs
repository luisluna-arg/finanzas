using FinanceApi.Application.Queries.Movements;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Movements;

public class GetAllMovementsQueryHandler : IRequestHandler<GetAllMovementsQuery, Movement[]>
{
    private readonly FinanceDbContext dbContext;

    public GetAllMovementsQueryHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<Movement[]> Handle(GetAllMovementsQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(await dbContext.Movement.ToArrayAsync());
    }
}
