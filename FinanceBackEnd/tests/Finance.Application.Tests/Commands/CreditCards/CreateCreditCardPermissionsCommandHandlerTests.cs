using Finance.Application.Auth;
using Finance.Application.Commands.CreditCards;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.Models.Identities;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.CreditCards;

public class CreateCreditCardPermissionsCommandHandlerTests : IDisposable
{
    private readonly FinanceDbContext _dbContext;

    public CreateCreditCardPermissionsCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task CreatePermissions_CreditCardAndUserFoundByClaimId_ReturnsPermissions()
    {
        const string userIdClaim = "auth0|test-user";
        var creditCardId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Username = "u",
            FirstName = "F",
            LastName = "L",
            Identities = [new Identity { SourceId = userIdClaim }],
        };
        await _dbContext.User.AddAsync(user);
        await _dbContext.CreditCard.AddAsync(new CreditCard { Id = creditCardId });
        await _dbContext.SaveChangesAsync();

        var command = new CreateCreditCardPermissionsCommand
        {
            ResourceId = creditCardId,
            PermissionLevels = [PermissionLevelEnum.Owner],
        };
        command.SetContext(new FinanceDispatchContext { UserIdClaim = userIdClaim });

        var handler = new CreateCreditCardPermissionsCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(creditCardId, result.Data.ResourceId);
        Assert.Equal(userId, result.Data.UserId);
        Assert.Contains(PermissionLevelEnum.Owner, result.Data.PermissionLevels);
    }

    [Fact]
    public async Task CreatePermissions_ExplicitUserId_FindsUserByIdAndReturnsPermissions()
    {
        var creditCardId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        await _dbContext.User.AddAsync(new User { Id = userId, Username = "u", FirstName = "F", LastName = "L" });
        await _dbContext.CreditCard.AddAsync(new CreditCard { Id = creditCardId });
        await _dbContext.SaveChangesAsync();

        var command = new CreateCreditCardPermissionsCommand
        {
            ResourceId = creditCardId,
            UserId = userId,
            PermissionLevels = [PermissionLevelEnum.Owner],
        };

        var handler = new CreateCreditCardPermissionsCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Data.UserId);
    }

    [Fact]
    public async Task CreatePermissions_CreditCardNotFound_ReturnsFailure()
    {
        var command = new CreateCreditCardPermissionsCommand
        {
            ResourceId = Guid.NewGuid(),
            PermissionLevels = [PermissionLevelEnum.Owner],
        };

        var handler = new CreateCreditCardPermissionsCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Resource not found", result.ErrorMessage);
    }

    [Fact]
    public async Task CreatePermissions_UserNotFound_ReturnsFailure()
    {
        var creditCardId = Guid.NewGuid();
        await _dbContext.CreditCard.AddAsync(new CreditCard { Id = creditCardId });
        await _dbContext.SaveChangesAsync();

        var command = new CreateCreditCardPermissionsCommand
        {
            ResourceId = creditCardId,
            PermissionLevels = [PermissionLevelEnum.Owner],
        };
        command.SetContext(new FinanceDispatchContext { UserIdClaim = "auth0|unknown" });

        var handler = new CreateCreditCardPermissionsCommandHandler(_dbContext);

        var result = await handler.ExecuteAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("User not found", result.ErrorMessage);
    }

    [Fact]
    public async Task CreatePermissions_PersistsPermissionRecord()
    {
        const string userIdClaim = "auth0|persist-test";
        var creditCardId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        await _dbContext.User.AddAsync(new User
        {
            Id = userId,
            Username = "u",
            FirstName = "F",
            LastName = "L",
            Identities = [new Identity { SourceId = userIdClaim }],
        });
        await _dbContext.CreditCard.AddAsync(new CreditCard { Id = creditCardId });
        await _dbContext.SaveChangesAsync();

        var command = new CreateCreditCardPermissionsCommand
        {
            ResourceId = creditCardId,
            PermissionLevels = [PermissionLevelEnum.Owner],
        };
        command.SetContext(new FinanceDispatchContext { UserIdClaim = userIdClaim });

        var handler = new CreateCreditCardPermissionsCommandHandler(_dbContext);
        await handler.ExecuteAsync(command);

        var saved = await _dbContext.CreditCardPermissions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.ResourceId == creditCardId && p.UserId == userId);

        Assert.NotNull(saved);
        Assert.Contains(PermissionLevelEnum.Owner, saved.PermissionLevels);
    }
}
