using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Users;

public class GetUserQuery() : GetSingleByIdQuery<User?, Guid>();

public class GetUserQueryHandler(FinanceDbContext db) : BaseResponseHandler<GetUserQuery, User?>(db)
{
    public override async Task<User?> Handle(GetUserQuery request, CancellationToken cancellationToken)
        => await DbContext.User.FirstOrDefaultAsync(o => o.Id == request.Id);
}
