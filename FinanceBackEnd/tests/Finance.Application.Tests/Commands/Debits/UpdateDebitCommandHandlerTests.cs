using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Debits;
using Finance.Application.Repositories;
using Finance.Domain.Enums;
using Finance.Domain.Models.Debits;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.Debits;

public class UpdateDebitCommandHandlerTests : IDisposable
{
    private readonly Mock<IRepository<Debit, Guid>> _debitRepo;
    private readonly Mock<IRepository<DebitOrigin, Guid>> _originRepo;
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly FinanceDbContext _dbContext;

    public UpdateDebitCommandHandlerTests()
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

    private UpdateDebitCommandHandler CreateHandler()
        => new(_dbContext, _debitRepo.Object, _originRepo.Object, _dispatcher.Object);

    [Fact]
    public async Task Update_ValidCommand_ReturnsUpdatedDebit()
    {
        var debitId = Guid.NewGuid();
        var origin = new DebitOrigin { Id = Guid.NewGuid(), Name = "Internet" };
        var debit = new Debit { Id = debitId, Amount = new Money(10m) };

        _debitRepo.Setup(r => r.GetByIdAsync(debitId, It.IsAny<CancellationToken>())).ReturnsAsync(debit);
        _originRepo.Setup(r => r.GetByAsync(It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>())).ReturnsAsync(origin);

        var command = new UpdateDebitCommand
        {
            Id = debitId,
            AppModuleId = Guid.NewGuid(),
            Origin = "Internet",
            Amount = new Money(200m),
            Frequency = FrequencyEnum.Monthly,
        };

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(new Money(200m), result.Data.Amount);
        Assert.Equal(origin, result.Data.Origin);
        Assert.Equal(FrequencyEnum.Monthly, result.Data.Frequency);
    }

    [Fact]
    public async Task Update_ValidCommand_CallsUpdateOnRepository()
    {
        var debitId = Guid.NewGuid();
        var debit = new Debit { Id = debitId, Amount = new Money(10m) };
        var origin = new DebitOrigin { Id = Guid.NewGuid(), Name = "Internet" };

        _debitRepo.Setup(r => r.GetByIdAsync(debitId, It.IsAny<CancellationToken>())).ReturnsAsync(debit);
        _originRepo.Setup(r => r.GetByAsync(It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>())).ReturnsAsync(origin);

        await CreateHandler().ExecuteAsync(new UpdateDebitCommand { Id = debitId, AppModuleId = Guid.NewGuid(), Origin = "Internet", Amount = new Money(200m) }, default);

        _debitRepo.Verify(r => r.UpdateAsync(It.IsAny<Debit>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task Update_DebitNotFound_ThrowsException()
    {
        _debitRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Debit?)null);

        var command = new UpdateDebitCommand { Id = Guid.NewGuid(), AppModuleId = Guid.NewGuid(), Origin = "X", Amount = new Money(100m) };

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Update_EmptyId_ThrowsValidationException()
    {
        var command = new UpdateDebitCommand { Id = Guid.Empty, AppModuleId = Guid.NewGuid(), Origin = "Rent", Amount = new Money(100m) };

        await Assert.ThrowsAsync<ValidationException>(() => CreateHandler().ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Update_ZeroAmount_ThrowsValidationException()
    {
        var command = new UpdateDebitCommand { Id = Guid.NewGuid(), AppModuleId = Guid.NewGuid(), Origin = "Rent", Amount = new Money(0m) };

        await Assert.ThrowsAsync<ValidationException>(() => CreateHandler().ExecuteAsync(command, default));
    }
}
