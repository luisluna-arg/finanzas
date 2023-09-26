using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.CreditCardIssuers;

public class GetCreditCardIssuersQueryHandler : BaseCollectionHandler<GetCreditCardIssuersQuery, CreditCardIssuer?>
{
    public GetCreditCardIssuersQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<CreditCardIssuer?>> Handle(GetCreditCardIssuersQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.CreditCardIssuers.Include(o => o.Bank).AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.BankId))
        {
            query = query.Where(o => o.BankId == new Guid(request.BankId));
        }

        return await query
            .OrderByDescending(o => o.Bank.Name)
            .OrderByDescending(o => o.Name)
            .ToArrayAsync();
    }
}

public class GetCreditCardIssuersQuery : GetAllQuery<CreditCardIssuer?>
{
    public string? BankId { get; private set; }
}
