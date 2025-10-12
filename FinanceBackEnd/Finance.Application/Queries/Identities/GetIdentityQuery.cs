using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Identities;

public class GetIdentityQuery() : GetSingleByIdQuery<Identity?, Guid>();

public class GetIdentityQueryHandler(FinanceDbContext db) : BaseQueryHandler<GetIdentityQuery, Identity?>(db)
{
    public override async Task<DataResult<Identity?>> ExecuteAsync(GetIdentityQuery request, CancellationToken cancellationToken)
        => DataResult<Identity?>.Success(await DbContext.Identity.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken));
}
