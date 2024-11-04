using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public class GetCreditCardMovementsQueryHandler : BaseCollectionHandler<GetCreditCardMovementsQuery, CreditCardMovement>
{
    public GetCreditCardMovementsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<CreditCardMovement>> Handle(GetCreditCardMovementsQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.CreditCardMovement.Include(o => o.CreditCard).ThenInclude(o => o.Bank).AsQueryable();

        if (request.CreditCardId.HasValue)
        {
            query = query.Where(o => o.CreditCardId == request.CreditCardId.Value);
        }

        if (request.From.HasValue)
        {
            query = query.FilterBy("TimeStamp", Application.Repositories.Base.ExpressionOperator.GreaterThanOrEqual, request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.FilterBy("TimeStamp", Application.Repositories.Base.ExpressionOperator.LessThanOrEqual, request.To.Value);
        }

        return await query
            .OrderBy(o => o.CreditCard.Name)
            .ThenBy(o => o.CreditCard.Bank.Name)
            .OrderByDescending(o => o.TimeStamp)
            .ToArrayAsync();
    }
}

public class GetCreditCardMovementsQuery : GetAllQuery<CreditCardMovement>
{
    public Guid? CreditCardId { get; set; }

    /// <summary>
    /// Gets or sets date to filter from. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? From { get; set; }

    /// <summary>
    /// Gets or sets date to filter to. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? To { get; set; }
}
