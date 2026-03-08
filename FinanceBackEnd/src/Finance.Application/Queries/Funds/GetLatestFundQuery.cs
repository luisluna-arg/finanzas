using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models.Funds;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Funds;

public class GetLatestFundQuery : IQuery<Fund?>
{
    private Guid bankId;

    public GetLatestFundQuery(Guid bankId)
    {
        this.bankId = bankId;
    }

    public Guid BankId { get => bankId; }
    public bool? DailyUse { get; set; }
}

public class GetLatestFundQueryHandler : BaseQueryHandler<GetLatestFundQuery, Fund?>
{
    private readonly IRepository<Fund, Guid> movementRepository;

    public GetLatestFundQueryHandler(
        FinanceDbContext db,
        IRepository<Fund, Guid> movementRepository)
        : base(db)
    {
        this.movementRepository = movementRepository;
    }

    public override async Task<DataResult<Fund?>> ExecuteAsync(GetLatestFundQuery request, CancellationToken cancellationToken)
    {
        var query = movementRepository.GetDbSet()
            .Include(o => o.Bank)
            .Include(o => o.Currency)
            .Where(o => o.BankId == request.BankId)
            .AsQueryable();

        if (request.DailyUse.HasValue)
        {
            query = query.Where(o => o.DailyUse == request.DailyUse.Value);
        }

        return DataResult<Fund?>.Success(await query
            .OrderByDescending(o => o.TimeStamp)
            .ThenByDescending(o => o.CreatedAt)
            .ThenBy(o => o.Id)
            .FirstOrDefaultAsync(cancellationToken));
    }
}
