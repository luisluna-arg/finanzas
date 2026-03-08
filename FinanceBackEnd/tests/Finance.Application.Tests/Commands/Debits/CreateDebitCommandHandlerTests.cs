using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Debits;
using Finance.Application.Legacy.Commands.DebitOrigins;
using Finance.Application.Repositories;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using Finance.Domain.Enums;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.Debits;

public class CreateDebitCommandHandlerTests : IDisposable
{
    private readonly Mock<IRepository<Debit, Guid>> _debitRepo;
    private readonly Mock<IRepository<DebitOrigin, Guid>> _originRepo;
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly FinanceDbContext _dbContext;

    public CreateDebitCommandHandlerTests()
    {
        _debitRepo = new Mock<IRepository<Debit, Guid>>();
        _originRepo = new Mock<IRepository<DebitOrigin, Guid>>();
        _dispatcher = new Mock<IDispatcher<FinanceDispatchContext>>();

        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
    }

    public void Dispose() => _dbContext.Dispose();

    private CreateDebitCommandHandler CreateHandler()
        => new(_dbContext, _debitRepo.Object, _originRepo.Object, _dispatcher.Object);

    private static CreateDebitCommand ValidCommand() => new()
    {
        AppModuleId = Guid.NewGuid(),
        Origin = "Rent",
        Amount = new Money(500m),
        Frequency = FrequencyEnum.Monthly,
    };

    [Fact]
    public async Task Create_ValidCommand_ReturnsSuccess()
    {
        var origin = new DebitOrigin { Id = Guid.NewGuid(), Name = "Rent" };
        var command = ValidCommand();

        _originRepo.Setup(r => r.GetByAsync(It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>())).ReturnsAsync(origin);
        _dispatcher.Setup(d => d.DispatchAsync<DataResult<DebitPermissions>>(It.IsAny<CreateDebitPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitPermissions>.Success(new DebitPermissions()));

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(origin, result.Data.Origin);
        Assert.Equal(command.Amount, result.Data.Amount);
        Assert.Equal(command.Frequency, result.Data.Frequency);
    }

    [Fact]
    public async Task Create_ValidCommand_AddsDebitToRepository()
    {
        var origin = new DebitOrigin { Id = Guid.NewGuid(), Name = "Rent" };
        _originRepo.Setup(r => r.GetByAsync(It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>())).ReturnsAsync(origin);
        _dispatcher.Setup(d => d.DispatchAsync<DataResult<DebitPermissions>>(It.IsAny<CreateDebitPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitPermissions>.Success(new DebitPermissions()));

        await CreateHandler().ExecuteAsync(ValidCommand(), default);

        _debitRepo.Verify(r => r.AddAsync(It.IsAny<Debit>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task Create_OriginNotFound_DispatchesCreateOriginCommand()
    {
        var appModuleId = Guid.NewGuid();
        var command = new CreateDebitCommand { AppModuleId = appModuleId, Origin = "  NewOrigin  ", Amount = new Money(100m) };

        _originRepo.Setup(r => r.GetByAsync(It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>())).ReturnsAsync((DebitOrigin?)null);
        _dispatcher.Setup(d => d.DispatchAsync<DataResult<DebitOrigin>>(It.IsAny<CreateDebitOriginCommand>()))
            .ReturnsAsync(DataResult<DebitOrigin>.Success(new DebitOrigin { Name = "NewOrigin" }));
        _dispatcher.Setup(d => d.DispatchAsync<DataResult<DebitPermissions>>(It.IsAny<CreateDebitPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitPermissions>.Success(new DebitPermissions()));

        await CreateHandler().ExecuteAsync(command, default);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<DebitOrigin>>(
            It.Is<CreateDebitOriginCommand>(c => c.Name == "NewOrigin" && c.AppModuleId == appModuleId)),
            Times.Once);
    }

    [Fact]
    public async Task Create_ValidCommand_DispatchesPermissionsWithOwner()
    {
        var origin = new DebitOrigin { Id = Guid.NewGuid(), Name = "Rent" };
        _originRepo.Setup(r => r.GetByAsync(It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>())).ReturnsAsync(origin);
        _dispatcher.Setup(d => d.DispatchAsync<DataResult<DebitPermissions>>(It.IsAny<CreateDebitPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitPermissions>.Success(new DebitPermissions()));

        await CreateHandler().ExecuteAsync(ValidCommand(), default);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<DebitPermissions>>(
            It.Is<CreateDebitPermissionsCommand>(c => c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_EmptyOrigin_ThrowsValidationException()
    {
        var command = new CreateDebitCommand { AppModuleId = Guid.NewGuid(), Origin = "", Amount = new Money(100m) };

        await Assert.ThrowsAsync<ValidationException>(() => CreateHandler().ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Create_ZeroAmount_ThrowsValidationException()
    {
        var command = new CreateDebitCommand { AppModuleId = Guid.NewGuid(), Origin = "Rent", Amount = new Money(0m) };

        await Assert.ThrowsAsync<ValidationException>(() => CreateHandler().ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Create_EmptyAppModuleId_ThrowsValidationException()
    {
        var command = new CreateDebitCommand { AppModuleId = Guid.Empty, Origin = "Rent", Amount = new Money(100m) };

        await Assert.ThrowsAsync<ValidationException>(() => CreateHandler().ExecuteAsync(command, default));
    }
}
