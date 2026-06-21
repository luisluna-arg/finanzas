using Finance.Application.Auth;
using Finance.Application.Commands.CreditCards;
using Finance.Application.Repositories;
using Finance.Application.Specifications.CreditCards;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Enums;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.Models.Identities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;

namespace Finance.Application.Tests.Commands.CreditCards;

public class CreateCreditCardStatementImportTemplateCommandHandlerTests : QueryHandlerBaseTests
{
    private readonly Mock<IRepository<CreditCardStatementImportTemplate, Guid>> _repository;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;

    public CreateCreditCardStatementImportTemplateCommandHandlerTests()
    {
        _repository = new Mock<IRepository<CreditCardStatementImportTemplate, Guid>>();
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
    }

    private CreateCreditCardStatementImportTemplateCommandHandler CreateHandler()
    {
        var isAdmin = new IsAdminUser(_httpContextAccessor.Object, _dbContext);
        return new(_repository.Object, _dbContext, isAdmin, new CanSetSystemFlag(isAdmin));
    }

    private async Task SetupAdminAsync()
    {
        var sourceId = Guid.NewGuid().ToString();
        var adminRole = await _dbContext.Role.FindAsync(RoleEnum.Admin)
                        ?? new Role { Id = RoleEnum.Admin, Name = "Admin" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            FirstName = "A",
            LastName = "B",
            Identities = [new Identity { SourceId = sourceId }],
            Roles = [adminRole],
        };
        await _dbContext.User.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        SetupIdentity(sourceId);
    }

    private void SetupIdentity(string? name)
    {
        var identity = new Mock<IIdentity>();
        identity.Setup(i => i.Name).Returns(name);
        var principal = new Mock<ClaimsPrincipal>();
        principal.Setup(p => p.Identity).Returns(identity.Object);
        var ctx = new Mock<HttpContext>();
        ctx.Setup(c => c.User).Returns(principal.Object);
        _httpContextAccessor.Setup(a => a.HttpContext).Returns(ctx.Object);
    }

    [Fact]
    public async Task Create_WithIsSystemTrue_CreatesTemplateWithNullUserId()
    {
        await SetupAdminAsync();

        var command = new CreateCreditCardStatementImportTemplateCommand
        {
            Name = "System Template",
            IsSystem = true,
            ConfigJson = "{}",
        };

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal("System Template", result.Data.Name);
        Assert.True(result.Data.IsSystem);
        Assert.Null(result.Data.UserId);
        _repository.Verify(
            r => r.AddAsync(It.IsAny<CreditCardStatementImportTemplate>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WithUserIdentity_SetsUserId()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "u",
            FirstName = "F",
            LastName = "L",
            Identities = [new Identity { SourceId = "IdentityNotFound" }],
        };
        await _dbContext.User.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        SetupIdentity("IdentityNotFound");

        var command = new CreateCreditCardStatementImportTemplateCommand
        {
            Name = "My Template",
            IsSystem = false,
            ConfigJson = "{}",
        };

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.False(result.Data.IsSystem);
        Assert.Equal(user.Id, result.Data.UserId);
    }

    [Fact]
    public async Task Create_WithNoHttpContext_CreatesTemplateWithNullUserId()
    {
        _httpContextAccessor.Setup(a => a.HttpContext).Returns((HttpContext?)null);

        var command = new CreateCreditCardStatementImportTemplateCommand
        {
            Name = "Anon Template",
            IsSystem = false,
            ConfigJson = "{}",
        };

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Data.UserId);
    }

    [Fact]
    public async Task Create_WithUnknownIdentity_CreatesTemplateWithNullUserId()
    {
        SetupIdentity("unknown-user-id");

        var command = new CreateCreditCardStatementImportTemplateCommand
        {
            Name = "Unknown User Template",
            IsSystem = false,
            ConfigJson = "{}",
        };

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Data.UserId);
    }

    [Fact]
    public async Task Create_SetsConfigJsonFromCommand()
    {
        await SetupAdminAsync();

        var configJson = "{\"skipRows\":2}";
        var command = new CreateCreditCardStatementImportTemplateCommand
        {
            Name = "Config Test",
            IsSystem = true,
            ConfigJson = configJson,
        };

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.Equal(configJson, result.Data.ConfigJson);
    }
}
