using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Users;

public class GetUserByIdQuery() : GetSingleByIdQuery<User?, Guid>();

public class GetUserByIdQueryHandler(FinanceDbContext db) : BaseQueryHandler<GetUserByIdQuery, User?>(db)
{
    public override async Task<DataResult<User?>> ExecuteAsync(GetUserByIdQuery request, CancellationToken cancellationToken)
        => DataResult<User?>.Success(await DbContext.User
            .Include(u => u.Roles)
            .Include(u => u.Identities)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken));
}
