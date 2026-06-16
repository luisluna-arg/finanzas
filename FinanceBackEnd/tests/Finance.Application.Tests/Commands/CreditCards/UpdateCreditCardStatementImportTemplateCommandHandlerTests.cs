using Finance.Application.Auth;
using Finance.Application.Commands.CreditCards;
using Finance.Application.Repositories;
using Finance.Application.Specifications.CreditCards;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.CreditCards;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Commands.CreditCards;

public class UpdateCreditCardStatementImportTemplateCommandHandlerTests : QueryHandlerBaseTests
{
    private readonly Mock<IRepository<CreditCardStatementImportTemplate, Guid>> _repository;

    public UpdateCreditCardStatementImportTemplateCommandHandlerTests()
    {
        _repository = new Mock<IRepository<CreditCardStatementImportTemplate, Guid>>();
    }

    private UpdateCreditCardStatementImportTemplateCommandHandler CreateHandler()
    {
        var isAdmin = new IsAdminUser(Mock.Of<IHttpContextAccessor>(), _dbContext);
        return new(_repository.Object, _dbContext, new CanSetSystemFlag(isAdmin));
    }

    [Fact]
    public async Task Update_WhenTemplateExists_UpdatesFieldsAndReturnsSuccess()
    {
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
