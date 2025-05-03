using Finance.Application.Base.Handlers;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Funds;

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
