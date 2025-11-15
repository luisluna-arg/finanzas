using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public class GetCreditCardQueryHandler : BaseCollectionHandler<GetCreditCardsQuery, CreditCard>
{
    public GetCreditCardQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<DataResult<List<CreditCard>>> ExecuteAsync(GetCreditCardsQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.CreditCard
            .Include(o => o.Bank)
            .Include(o => o.Transactions)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.BankId))
        {
            query = query.Where(o => o.BankId == new Guid(request.BankId));
        }

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return DataResult<List<CreditCard>>.Success(
            await query
            .OrderBy(o => o.Bank.Name)
            .ThenBy(o => o.Name)
            .ToListAsync(cancellationToken)
        );
    }
}

public class GetCreditCardsQuery : GetAllQuery<CreditCard>
{
    public string? BankId { get; private set; }
}
