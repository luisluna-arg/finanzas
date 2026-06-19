using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public class GetCreditCardStatementImportTemplatesQueryHandler : BaseCollectionQueryHandler<GetCreditCardStatementImportTemplatesQuery, CreditCardStatementImportTemplate>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetCreditCardStatementImportTemplatesQueryHandler(FinanceDbContext db, IHttpContextAccessor httpContextAccessor)
        : base(db)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<DataResult<List<CreditCardStatementImportTemplate>>> ExecuteAsync(
        GetCreditCardStatementImportTemplatesQuery request, CancellationToken cancellationToken)
    {
        var identitySourceId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        var user = !string.IsNullOrEmpty(identitySourceId)
            ? await DbContext.User
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Include(u => u.Identities)
                .FirstOrDefaultAsync(u => u.Identities.Any(i => i.SourceId == identitySourceId), cancellationToken)
            : null;

        var query = DbContext.CreditCardStatementImportTemplate
            .IgnoreQueryFilters()
            .Where(t => !t.Deactivated)
            .AsQueryable();

        if (request.CreditCardId.HasValue)
        {
            query = query.Where(t => t.IsSystem || t.CreditCards.Any(c => c.Id == request.CreditCardId.Value));
        }
        else
        {
            query = user != null
                ? query.Where(t => t.IsSystem || t.UserId == user.Id)
                : query.Where(t => t.IsSystem);
        }

        var results = await query
            .OrderBy(t => t.IsSystem ? 0 : 1)
            .ThenBy(t => t.Name)
            .ToListAsync(cancellationToken);

        return DataResult<List<CreditCardStatementImportTemplate>>.Success(results);
    }
}

public class GetCreditCardStatementImportTemplatesQuery : GetAllQuery<CreditCardStatementImportTemplate>
{
    public Guid? CreditCardId { get; set; }
}
