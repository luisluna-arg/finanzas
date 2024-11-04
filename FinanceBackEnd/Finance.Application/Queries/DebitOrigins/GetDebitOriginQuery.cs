using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.DebitOrigins;

public class GetDebitOriginQueryHandler : BaseResponseHandler<GetDebitOriginQuery, DebitOrigin?>
{
    public GetDebitOriginQueryHandler(
        FinanceDbContext db,
        IRepository<DebitOrigin, Guid> bankRepository)
        : base(db)
    {
    }

    public override async Task<DebitOrigin?> Handle(GetDebitOriginQuery request, CancellationToken cancellationToken)
        => await this.DbContext.DebitOrigin
            .Include(o => o.AppModule)
            .FirstOrDefaultAsync(o => o.Id == request.Id);
}

public class GetDebitOriginQuery : GetSingleByIdQuery<DebitOrigin?, Guid>
{
}
