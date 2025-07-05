using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Roles;

public class GetRoleQuery() : GetSingleByIdQuery<Role?, RoleEnum>();

public class GetRoleQueryHandler(FinanceDbContext db) : BaseResponseHandler<GetRoleQuery, Role?>(db)
{
    public override async Task<Role?> Handle(GetRoleQuery request, CancellationToken cancellationToken)
        => await DbContext.Role.FirstOrDefaultAsync(o => o.Id == request.Id);
}
