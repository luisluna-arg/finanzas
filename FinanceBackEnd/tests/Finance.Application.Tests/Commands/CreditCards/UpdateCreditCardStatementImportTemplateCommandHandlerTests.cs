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

public class UpdateCreditCardStatementImportTemplateCommandHandlerTests : QueryHandlerBaseTests
{
    private readonly Mock<IRepository<CreditCardStatementImportTemplate, Guid>> _repository;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;

    public UpdateCreditCardStatementImportTemplateCommandHandlerTests()
    {
        _repository = new Mock<IRepository<CreditCardStatementImportTemplate, Guid>>();
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
    }

    private UpdateCreditCardStatementImportTemplateCommandHandler CreateHandler()
    {
        var isAdmin = new IsAdminUser(_httpContextAccessor.Object, _dbContext);
        return new(_repository.Object, _dbContext, new CanSetSystemFlag(isAdmin));
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

        var identity = new Mock<IIdentity>();
        identity.Setup(i => i.Name).Returns(sourceId);
        var principal = new Mock<ClaimsPrincipal>();
        principal.Setup(p => p.Identity).Returns(identity.Object);
        var ctx = new Mock<HttpContext>();
        ctx.Setup(c => c.User).Returns(principal.Object);
        _httpContextAccessor.Setup(a => a.HttpContext).Returns(ctx.Object);
    }

    [Fact]
    public async Task Update_WhenTemplateExists_UpdatesFieldsAndReturnsSuccess()
    {
        await SetupAdminAsync();

        var existing = new CreditCardStatementImportTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            IsSystem = false,
            ConfigJson = "{\"skipRows\":1}",
        };

        _repository
            .Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var command = new UpdateCreditCardStatementImportTemplateCommand
        {
            Id = existing.Id,
            Name = "New Name",
            IsSystem = true,
            ConfigJson = "{\"skipRows\":2}",
        };

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", result.Data.Name);
        Assert.True(result.Data.IsSystem);
        Assert.Equal("{\"skipRows\":2}", result.Data.ConfigJson);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<CreditCardStatementImportTemplate>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
        _repository.Verify(r => r.PersistAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_WhenTemplateNotFound_ThrowsException()
    {
        var id = Guid.NewGuid();
        _repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreditCardStatementImportTemplate?)null);

        var command = new UpdateCreditCardStatementImportTemplateCommand
        {
            Id = id,
            Name = "X",
            IsSystem = false,
            ConfigJson = "{}",
        };

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Update_PreservesUnchangedProperties()
    {
        var userId = Guid.NewGuid();
        var existing = new CreditCardStatementImportTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Original",
            IsSystem = false,
            UserId = userId,
            ConfigJson = "{}",
        };

        _repository
            .Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var command = new UpdateCreditCardStatementImportTemplateCommand
        {
            Id = existing.Id,
            Name = "Updated",
            IsSystem = false,
            ConfigJson = "{}",
        };

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.Equal(userId, result.Data.UserId);
    }
}
