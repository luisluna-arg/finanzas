using CQRSDispatch;
using Finance.Application.Base.Handlers;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Incomes;

public class GetLatestIncomeQueryHandler : BaseQueryHandler<GetLatestIncomeQuery, Income?>
{
    private readonly IRepository<Income, Guid> movementRepository;

    public GetLatestIncomeQueryHandler(
        FinanceDbContext db,
        IRepository<Income, Guid> movementRepository)
        : base(db)
    {
        this.movementRepository = movementRepository;
    }

    public override async Task<DataResult<Income?>> ExecuteAsync(GetLatestIncomeQuery request, CancellationToken cancellationToken)
    {
        var query = movementRepository.GetDbSet()
            .Include(o => o.Bank)
            .Include(o => o.Currency)
            .AsQueryable();

        return DataResult<Income?>.Success(await query.FirstOrDefaultAsync(o => o.CurrencyId == request.BankId, cancellationToken));
    }
}

public class GetLatestIncomeQuery : IQuery<Income?>
{
    private Guid bankId;

    public GetLatestIncomeQuery(Guid bankId)
    {
        this.bankId = bankId;
    }

    public Guid BankId { get => bankId; }
}
