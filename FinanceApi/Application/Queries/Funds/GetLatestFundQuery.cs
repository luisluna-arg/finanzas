using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.Funds;

public class GetLatestFundQueryHandler : BaseResponseHandler<GetLatestFundQuery, Fund?>
{
    private readonly IRepository<Fund, Guid> movementRepository;

    public GetLatestFundQueryHandler(
        FinanceDbContext db,
        IRepository<Fund, Guid> movementRepository)
        : base(db)
    {
        this.movementRepository = movementRepository;
    }

    public override async Task<Fund?> Handle(GetLatestFundQuery request, CancellationToken cancellationToken)
    {
        var query = movementRepository.GetDbSet()
            .Include(o => o.Bank)
            .Include(o => o.Currency)
            .AsQueryable();

        if (request.DailyUse.HasValue)
        {
            query = query.Where(o => o.DailyUse == request.DailyUse.Value);
        }

        return await query.FirstOrDefaultAsync(o => o.CurrencyId == request.BankId);
    }
}

public class GetLatestFundQuery : IRequest<Fund>
{
    private Guid bankId;

    public GetLatestFundQuery(Guid bankId)
    {
        this.bankId = bankId;
    }

    public Guid BankId { get => bankId; }

    public bool? DailyUse { get; set; }
}
