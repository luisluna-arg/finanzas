using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.CreditCards;

public class GetCreditCardQueryHandler : BaseCollectionHandler<GetCreditCardsQuery, CreditCard?>
{
    public GetCreditCardQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<CreditCard?>> Handle(GetCreditCardsQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.CreditCard.Include(o => o.Bank).AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.BankId))
        {
            query = query.Where(o => o.BankId == new Guid(request.BankId));
        }

        return await query
            .OrderBy(o => o.Bank.Name)
            .ThenBy(o => o.Name)
            .ToArrayAsync();
    }
}

public class GetCreditCardsQuery : GetAllQuery<CreditCard?>
{
    public string? BankId { get; private set; }
}
