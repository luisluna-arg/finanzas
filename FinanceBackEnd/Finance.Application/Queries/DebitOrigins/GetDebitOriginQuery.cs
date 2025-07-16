using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.DebitOrigins;

public class GetDebitOriginQuery : GetSingleByIdQuery<DebitOrigin?, Guid>;

public class GetDebitOriginQueryHandler(FinanceDbContext db) : BaseQueryHandler<GetDebitOriginQuery, DebitOrigin?>(db)
{
    public override async Task<DataResult<DebitOrigin?>> ExecuteAsync(GetDebitOriginQuery request, CancellationToken cancellationToken)
        => DataResult<DebitOrigin?>.Success(
            await DbContext.DebitOrigin
            .Include(o => o.AppModule)
            .FirstOrDefaultAsync(o => o.Id == request.Id));
}
