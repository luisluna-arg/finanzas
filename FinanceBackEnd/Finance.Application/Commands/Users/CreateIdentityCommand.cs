using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models.Identities;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands;

public class CreateIdentityCommand : BaseIdentityCommand;

public class CreateIdentityCommandHandler : BaseCommandHandler<CreateIdentityCommand, Identity>
{
    public CreateIdentityCommandHandler(FinanceDbContext dbContext) : base(dbContext) { }

    public async override Task<DataResult<Identity>> ExecuteAsync(CreateIdentityCommand request, CancellationToken cancellationToken = default)
    {
        var user = await DbContext.User.Include(u => u.Identities).FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user == null) return DataResult<Identity>.Failure("User not found");

        var identity = new Identity
        {
            UserId = request.UserId,
            Provider = request.Provider,
            SourceId = request.SourceId
        };

        user.Identities.Add(identity);

        await DbContext.SaveChangesAsync(cancellationToken);

        return DataResult<Identity>.Success(identity);
    }
}
