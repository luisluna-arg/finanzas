using Finance.Application.Commands.DebitOrigins;
using Finance.Application.Repositories;
using Finance.Domain.Models.AppModules;
using Finance.Domain.Models.Debits;
using FluentValidation;
using Finance.Application.Tests.Queries.Base;

namespace Finance.Application.Tests.Commands.DebitOrigins;

public class CreateDebitOriginCommandHandlerTests : QueryHandlerBaseTests
{
    private readonly Mock<IRepository<AppModule, Guid>> _appModuleRepo;
    private readonly Mock<IRepository<DebitOrigin, Guid>> _debitOriginRepo;

    public CreateDebitOriginCommandHandlerTests()
    {
        _appModuleRepo = new Mock<IRepository<AppModule, Guid>>();
        _debitOriginRepo = new Mock<IRepository<DebitOrigin, Guid>>();
    }

    private CreateDebitOriginCommandHandler CreateHandler()
        => new(_dbContext, _appModuleRepo.Object, _debitOriginRepo.Object);

    private static CreateDebitOriginCommand ValidCommand() => new()
    {
        AppModuleId = Guid.NewGuid(),
        Name = "Streaming",
    };

    [Fact]
    public async Task Create_ValidCommand_ReturnsSuccess()
    {
        var appModule = new AppModule { Id = Guid.NewGuid(), Name = "Personal" };
        var command = ValidCommand();
        command.AppModuleId = appModule.Id;

        _appModuleRepo.Setup(r => r.GetByIdAsync(appModule.Id, It.IsAny<CancellationToken>())).ReturnsAsync(appModule);

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal("Streaming", result.Data.Name);
        Assert.Equal(appModule, result.Data.AppModule);
    }

    [Fact]
    public async Task Create_ValidCommand_AddsOriginToRepository()
    {
        var appModule = new AppModule { Id = Guid.NewGuid(), Name = "Personal" };
        var command = ValidCommand();
        command.AppModuleId = appModule.Id;

        _appModuleRepo.Setup(r => r.GetByIdAsync(appModule.Id, It.IsAny<CancellationToken>())).ReturnsAsync(appModule);

        await CreateHandler().ExecuteAsync(command, default);

        _debitOriginRepo.Verify(r => r.AddAsync(It.IsAny<DebitOrigin>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task Create_TrimsName()
    {
        var appModule = new AppModule { Id = Guid.NewGuid(), Name = "Personal" };
        var command = new CreateDebitOriginCommand { AppModuleId = appModule.Id, Name = "  Netflix  " };

        _appModuleRepo.Setup(r => r.GetByIdAsync(appModule.Id, It.IsAny<CancellationToken>())).ReturnsAsync(appModule);

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.Equal("Netflix", result.Data.Name);
    }

    [Fact]
    public async Task Create_AppModuleNotFound_ThrowsException()
    {
        _appModuleRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((AppModule?)null);

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().ExecuteAsync(ValidCommand(), default));
    }

    [Fact]
    public async Task Create_EmptyAppModuleId_ThrowsValidationException()
    {
        var command = new CreateDebitOriginCommand { AppModuleId = Guid.Empty, Name = "Streaming" };

        await Assert.ThrowsAsync<ValidationException>(() => CreateHandler().ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Create_EmptyName_ThrowsValidationException()
    {
        var command = new CreateDebitOriginCommand { AppModuleId = Guid.NewGuid(), Name = "" };

        await Assert.ThrowsAsync<ValidationException>(() => CreateHandler().ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Create_NameTooLong_ThrowsValidationException()
    {
        var command = new CreateDebitOriginCommand { AppModuleId = Guid.NewGuid(), Name = new string('x', 201) };

        await Assert.ThrowsAsync<ValidationException>(() => CreateHandler().ExecuteAsync(command, default));
    }
}
