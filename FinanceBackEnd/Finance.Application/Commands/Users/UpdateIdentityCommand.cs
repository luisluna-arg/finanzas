using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models.Identities;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands;

public class UpdateIdentityCommand : BaseIdentityCommand
{
    public Guid IdentityId { get; internal set; }
}

public class UpdateIdentityCommandHandler : BaseCommandHandler<UpdateIdentityCommand, Identity>
{
    public UpdateIdentityCommandHandler(FinanceDbContext dbContext) : base(dbContext) { }

    public async override Task<DataResult<Identity>> ExecuteAsync(UpdateIdentityCommand request, CancellationToken cancellationToken = default)
    {
        var user = await DbContext.User.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user == null)
        {
            return DataResult<Identity>.Failure("User not found");
        }

        var existingIdentity = await DbContext.Identity.FirstOrDefaultAsync(i => i.Id == request.IdentityId && i.UserId == request.UserId, cancellationToken);
        if (existingIdentity == null)
        {
            return DataResult<Identity>.Failure("Identity not found");
        }

        existingIdentity.Provider = request.Provider;
        existingIdentity.SourceId = request.SourceId;

        await DbContext.SaveChangesAsync(cancellationToken);

        return DataResult<Identity>.Success(existingIdentity);
    }
}
