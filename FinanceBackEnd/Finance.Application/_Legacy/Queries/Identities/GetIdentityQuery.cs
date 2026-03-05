using CQRSDispatch;
using Finance.Application.Legacy.Base.Handlers;
using Finance.Application.Legacy.Queries.Base;
using Finance.Domain.Models.Identities;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Legacy.Queries.Identities;

public class GetIdentityQuery() : GetSingleByIdQuery<Identity?, Guid>();

public class GetIdentityQueryHandler(FinanceDbContext db) : BaseQueryHandler<GetIdentityQuery, Identity?>(db)
{
    public override async Task<DataResult<Identity?>> ExecuteAsync(GetIdentityQuery request, CancellationToken cancellationToken)
        => DataResult<Identity?>.Success(await DbContext.Identity.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken));
}
