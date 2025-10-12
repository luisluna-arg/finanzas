using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Users;

public class GetUserBySourceIdsQuery : ContextAwareQuery<User?>
{
    public string[] SourceIds { get; }

    public GetUserBySourceIdsQuery(string[] sourceIds)
    {
        SourceIds = sourceIds;
    }
}

public class GetUserBySourceIdsQueryHandler : IQueryHandler<GetUserBySourceIdsQuery, User?>
{
    public FinanceDbContext DbContext { get; }

    public GetUserBySourceIdsQueryHandler(FinanceDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<DataResult<User?>> ExecuteAsync(GetUserBySourceIdsQuery request, CancellationToken cancellationToken = default)
    {
        var user = await DbContext.User
            .Include(u => u.Roles)
            .Include(u => u.Identities)
            .FirstOrDefaultAsync(u => u.Identities.Any(i => request.SourceIds.Contains(i.SourceId)), cancellationToken);

        if (user == null)
        {
            return DataResult<User?>.Failure("User not found with the provided source IDs.");
        }

        return DataResult<User?>.Success(user);
    }
}