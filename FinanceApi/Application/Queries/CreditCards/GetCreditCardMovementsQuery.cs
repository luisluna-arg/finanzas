using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.CreditCardMovements;

public class GetCreditCardMovementsQueryHandler : BaseCollectionHandler<GetCreditCardMovementsQuery, CreditCardMovement>
{
    public GetCreditCardMovementsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<CreditCardMovement>> Handle(GetCreditCardMovementsQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.CreditCardMovements.Include(o => o.CreditCardIssuer).ThenInclude(o => o.Bank).AsQueryable();

        if (request.IssuerId.HasValue)
        {
            query = query.Where(o => o.CreditCardIssuerId == request.IssuerId.Value);
        }

        if (request.From.HasValue)
        {
            query = query.FilterBy("TimeStamp", Infrastructure.Repositories.Base.ExpressionOperator.GreaterThanOrEqual, request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.FilterBy("TimeStamp", Infrastructure.Repositories.Base.ExpressionOperator.LessThanOrEqual, request.To.Value);
        }

        return await query.OrderByDescending(o => o.TimeStamp).ThenBy(o => o.CreditCardIssuer.Name).ToArrayAsync();
    }
}

public class GetCreditCardMovementsQuery : GetAllQuery<CreditCardMovement>
{
    public Guid? IssuerId { get; set; }

    /// <summary>
    /// Gets or sets date to filter from. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? From { get; set; }

    /// <summary>
    /// Gets or sets date to filter to. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? To { get; set; }
}
