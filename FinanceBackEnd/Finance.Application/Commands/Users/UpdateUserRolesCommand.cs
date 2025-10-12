using System.Text.Json.Serialization;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Enums;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.Users;

public class UpdateUserRolesCommand : ICommand
{
    public UpdateUserRolesCommand(Guid userId, ICollection<RoleEnum> roles)
    {
        UserId = userId;
        Roles = roles ?? [];
    }

    [JsonIgnore]
    public Guid UserId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ICollection<RoleEnum> Roles { get; set; }
}


public class UpdateUserRolesCommandHandler : ICommandHandler<UpdateUserRolesCommand, CommandResult>
{
    public UpdateUserRolesCommandHandler(FinanceDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public FinanceDbContext DbContext { get; }

    public async Task<CommandResult> ExecuteAsync(UpdateUserRolesCommand command, CancellationToken cancellationToken)
    {
        var user = await DbContext.User
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
        if (user == null)
        {
            return CommandResult.Failure("User not found.");
        }

        var roles = DbContext.Role.Where(r => command.Roles.Contains(r.Id)).ToList();
        if (roles.Count != command.Roles.Count)
        {
            return CommandResult.Failure("Some roles not found.");
        }

        // Remove roles not in the new set
        var rolesToRemove = user.Roles.Where(r => !roles.Contains(r)).ToList();
        foreach (var role in rolesToRemove)
        {
            user.Roles.Remove(role);
        }

        // Add roles that are not already present
        foreach (var role in roles)
        {
            if (!user.Roles.Contains(role))
            {
                user.Roles.Add(role);
            }
        }

        DbContext.User.Update(user);
        await DbContext.SaveChangesAsync(cancellationToken);

        return CommandResult.Success();
    }
}