using Finance.Application.Base.Handlers;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Incomes;

public class GetLatestIncomeQueryHandler : BaseResponseHandler<GetLatestIncomeQuery, Income?>
{
    private readonly IRepository<Income, Guid> movementRepository;

    public GetLatestIncomeQueryHandler(
        FinanceDbContext db,
        IRepository<Income, Guid> movementRepository)
        : base(db)
    {
        this.movementRepository = movementRepository;
    }

    public override async Task<Income?> Handle(GetLatestIncomeQuery request, CancellationToken cancellationToken)
    {
        var query = movementRepository.GetDbSet()
            .Include(o => o.Bank)
            .Include(o => o.Currency)
            .AsQueryable();

        return await query.FirstOrDefaultAsync(o => o.CurrencyId == request.BankId);
    }
}

public class GetLatestIncomeQuery : IRequest<Income>
{
    private Guid bankId;

    public GetLatestIncomeQuery(Guid bankId)
    {
        this.bankId = bankId;
    }

    public Guid BankId { get => bankId; }
}
