using Finance.Application.Commands.DebitOrigins;
using Finance.Application.Repositories;
using Finance.Domain.Models.AppModules;
using Finance.Domain.Models.Debits;
using Finance.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.DebitOrigins;

public class UpdateDebitOriginCommandHandlerTests : IDisposable
{
    private readonly Mock<IRepository<AppModule, Guid>> _appModuleRepo;
    private readonly Mock<IRepository<DebitOrigin, Guid>> _debitOriginRepo;
    private readonly FinanceDbContext _dbContext;

    public UpdateDebitOriginCommandHandlerTests()
    {
        _appModuleRepo = new Mock<IRepository<AppModule, Guid>>();
        _debitOriginRepo = new Mock<IRepository<DebitOrigin, Guid>>();

        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
    }

    public void Dispose() => _dbContext.Dispose();

    private UpdateDebitOriginCommandHandler CreateHandler()
        => new(_dbContext, _appModuleRepo.Object, _debitOriginRepo.Object);

    [Fact]
    public async Task Update_ValidCommand_ReturnsUpdatedOrigin()
    {
        var originId = Guid.NewGuid();
        var appModule = new AppModule { Id = Guid.NewGuid(), Name = "Personal" };
        var origin = new DebitOrigin { Id = originId, Name = "OldName" };

        _debitOriginRepo.Setup(r => r.GetByIdAsync(originId, It.IsAny<CancellationToken>())).ReturnsAsync(origin);
        _appModuleRepo.Setup(r => r.GetByIdAsync(appModule.Id, It.IsAny<CancellationToken>())).ReturnsAsync(appModule);

        var command = new UpdateDebitOriginCommand { Id = originId, AppModuleId = appModule.Id, Name = "Netflix" };

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal("Netflix", result.Data.Name);
        Assert.Equal(appModule, result.Data.AppModule);
    }

    [Fact]
    public async Task Update_ValidCommand_CallsUpdateOnRepository()
    {
        var originId = Guid.NewGuid();
        var appModule = new AppModule { Id = Guid.NewGuid(), Name = "Personal" };
        var origin = new DebitOrigin { Id = originId, Name = "OldName" };

        _debitOriginRepo.Setup(r => r.GetByIdAsync(originId, It.IsAny<CancellationToken>())).ReturnsAsync(origin);
        _appModuleRepo.Setup(r => r.GetByIdAsync(appModule.Id, It.IsAny<CancellationToken>())).ReturnsAsync(appModule);

        await CreateHandler().ExecuteAsync(new UpdateDebitOriginCommand { Id = originId, AppModuleId = appModule.Id, Name = "Netflix" }, default);

        _debitOriginRepo.Verify(r => r.UpdateAsync(It.IsAny<DebitOrigin>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task Update_TrimsName()
    {
        var originId = Guid.NewGuid();
        var appModule = new AppModule { Id = Guid.NewGuid(), Name = "Personal" };
        var origin = new DebitOrigin { Id = originId, Name = "OldName" };

        _debitOriginRepo.Setup(r => r.GetByIdAsync(originId, It.IsAny<CancellationToken>())).ReturnsAsync(origin);
        _appModuleRepo.Setup(r => r.GetByIdAsync(appModule.Id, It.IsAny<CancellationToken>())).ReturnsAsync(appModule);

        var result = await CreateHandler().ExecuteAsync(new UpdateDebitOriginCommand { Id = originId, AppModuleId = appModule.Id, Name = "  Spotify  " }, default);

        Assert.Equal("Spotify", result.Data.Name);
    }

    [Fact]
    public async Task Update_OriginNotFound_ThrowsException()
    {
        _debitOriginRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((DebitOrigin?)null);

        await Assert.ThrowsAsync<Exception>(() =>
            CreateHandler().ExecuteAsync(new UpdateDebitOriginCommand { Id = Guid.NewGuid(), AppModuleId = Guid.NewGuid(), Name = "X" }, default));
    }

    [Fact]
    public async Task Update_EmptyId_ThrowsValidationException()
    {
        var command = new UpdateDebitOriginCommand { Id = Guid.Empty, AppModuleId = Guid.NewGuid(), Name = "X" };

        await Assert.ThrowsAsync<ValidationException>(() => CreateHandler().ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Update_EmptyName_ThrowsValidationException()
    {
        var command = new UpdateDebitOriginCommand { Id = Guid.NewGuid(), AppModuleId = Guid.NewGuid(), Name = "" };

        await Assert.ThrowsAsync<ValidationException>(() => CreateHandler().ExecuteAsync(command, default));
    }
}
