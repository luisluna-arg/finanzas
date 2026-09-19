using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public class GetCreditCardInstallmentPatternsQueryHandler
    : BaseCollectionQueryHandler<GetCreditCardInstallmentPatternsQuery, CreditCardInstallmentPattern>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetCreditCardInstallmentPatternsQueryHandler(FinanceDbContext db, IHttpContextAccessor httpContextAccessor)
        : base(db)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<DataResult<List<CreditCardInstallmentPattern>>> ExecuteAsync(
        GetCreditCardInstallmentPatternsQuery request, CancellationToken cancellationToken)
    {
        var identitySourceId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        var user = !string.IsNullOrEmpty(identitySourceId)
            ? await DbContext.User
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Include(u => u.Identities)
                .FirstOrDefaultAsync(u => u.Identities.Any(i => i.SourceId == identitySourceId), cancellationToken)
            : null;

        var query = DbContext.CreditCardInstallmentPattern
            .IgnoreQueryFilters()
            .Where(p => !p.Deactivated)
            .AsQueryable();

        query = user != null
            ? query.Where(p => p.IsSystem || p.UserId == user.Id)
            : query.Where(p => p.IsSystem);

        var results = await query
            .OrderBy(p => p.IsSystem ? 0 : 1)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return DataResult<List<CreditCardInstallmentPattern>>.Success(results);
    }
}

public class GetCreditCardInstallmentPatternsQuery : GetAllQuery<CreditCardInstallmentPattern>;
