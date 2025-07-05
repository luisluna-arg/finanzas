using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Identities;

public class GetIdentityQuery() : GetSingleByIdQuery<Identity?, Guid>();

public class GetIdentityQueryHandler(FinanceDbContext db) : BaseResponseHandler<GetIdentityQuery, Identity?>(db)
{
    public override async Task<Identity?> Handle(GetIdentityQuery request, CancellationToken cancellationToken)
        => await DbContext.Identity.FirstOrDefaultAsync(o => o.Id == request.Id);
}
