using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Incomes;
using Finance.Application.Services;
using Finance.Application.Services.Incomes;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Incomes;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Services.Incomes;

public class IncomeServiceTests : IDisposable
{
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly FinanceDbContext _dbContext;
    private readonly IncomeService _sut;

    public IncomeServiceTests()
    {
        _dispatcher = new Mock<IDispatcher<FinanceDispatchContext>>();

        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
        _sut = new IncomeService(_dispatcher.Object, _dbContext);
    }

    public void Dispose() => _dbContext.Dispose();

    #region Create

    [Fact]
    public async Task Create_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var income = new Income { Id = Guid.NewGuid() };
        var request = new CreateIncomeRequest(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(100m));

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Income>>(It.IsAny<CreateIncomeCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Income>.Success(income));

        var result = await _sut.Create(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(income, result.Data);
    }

    [Fact]
    public async Task Create_WhenDispatchSucceeds_DispatchesCommandWithCorrectProperties()
    {
        var bankId = Guid.NewGuid();
        var currencyId = Guid.NewGuid();
        var timeStamp = DateTime.UtcNow;
        var amount = new Money(250m);
        var request = new CreateIncomeRequest(bankId, currencyId, timeStamp, amount);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Income>>(It.IsAny<CreateIncomeCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Income>.Success(new Income()));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<Income>>(
            It.Is<CreateIncomeCommand>(c =>
                c.BankId == bankId &&
                c.CurrencyId == currencyId &&
                c.TimeStamp == timeStamp &&
                c.Amount == amount),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenDispatchFails_ReturnsFailure()
    {
        var request = new CreateIncomeRequest(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(100m));

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Income>>(It.IsAny<CreateIncomeCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Income>.Failure("bank not found"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("bank not found", result.ErrorMessage);
    }

    [Fact]
    public async Task Create_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new CreateIncomeRequest(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(100m));

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Income>>(It.IsAny<CreateIncomeCommand>(), It.IsAny<HttpRequest?>()))
            .Throws(new Exception("unexpected error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected error", result.ErrorMessage);
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var income = new Income { Id = Guid.NewGuid() };
        var request = new UpdateIncomeRequest(income.Id, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(150m));

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Income>>(It.IsAny<UpdateIncomeCommand>()))
            .ReturnsAsync(DataResult<Income>.Success(income));

        var result = await _sut.Update(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(income, result.Data);
    }

    [Fact]
    public async Task Update_DispatchesCommandWithCorrectProperties()
    {
        var incomeId = Guid.NewGuid();
        var bankId = Guid.NewGuid();
        var currencyId = Guid.NewGuid();
        var timeStamp = DateTime.UtcNow;
        var amount = new Money(500m);
        var request = new UpdateIncomeRequest(incomeId, bankId, currencyId, timeStamp, amount);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Income>>(It.IsAny<UpdateIncomeCommand>()))
            .ReturnsAsync(DataResult<Income>.Success(new Income()));

        await _sut.Update(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<Income>>(
            It.Is<UpdateIncomeCommand>(c =>
                c.Id == incomeId &&
                c.BankId == bankId &&
                c.CurrencyId == currencyId &&
                c.TimeStamp == timeStamp &&
                c.Amount == amount)),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenDispatchFails_ReturnsFailure()
    {
        var request = new UpdateIncomeRequest(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(100m));

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Income>>(It.IsAny<UpdateIncomeCommand>()))
            .ReturnsAsync(DataResult<Income>.Failure("income not found"));

        var result = await _sut.Update(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("income not found", result.ErrorMessage);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var request = new DeleteIncomeRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIncomesCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIncomeOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Delete(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_DeletesOwnerForEachId()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var request = new DeleteIncomeRequest([id1, id2]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIncomesCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIncomeOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteIncomeOwnerCommand>(c => c.EntityId == id1),
            It.IsAny<HttpRequest?>()),
            Times.Once);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteIncomeOwnerCommand>(c => c.EntityId == id2),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Delete_WhenDeleteCommandFails_ReturnsFailureAndSkipsOwnerDeletion()
    {
        var request = new DeleteIncomeRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIncomesCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Failure("delete failed"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("Type Income delete operation failed", result.ErrorMessage);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteIncomeOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new DeleteIncomeRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIncomesCommand>(), It.IsAny<HttpRequest?>()))
            .Throws(new Exception("unexpected error"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected error", result.ErrorMessage);
    }

    #endregion

    #region SetOwner

    [Fact]
    public async Task SetOwner_DispatchesCreatePermissionsCommandWithCorrectResourceId()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IncomePermissions>>(It.IsAny<CreateIncomePermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<IncomePermissions>.Success(new IncomePermissions()));

        await _sut.SetOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<IncomePermissions>>(
            It.Is<CreateIncomePermissionsCommand>(c =>
                c.ResourceId == resourceId &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task SetOwner_ReturnsDispatcherResult()
    {
        var permissions = new IncomePermissions();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<IncomePermissions>>(It.IsAny<CreateIncomePermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<IncomePermissions>.Success(permissions));

        var result = await _sut.SetOwner(Guid.NewGuid());

        Assert.True(result.IsSuccess);
        Assert.Equal(permissions, result.Data);
    }

    #endregion

    #region DeleteOwner

    [Fact]
    public async Task DeleteOwner_DispatchesDeleteOwnerCommandWithCorrectEntityId()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIncomeOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.DeleteOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteIncomeOwnerCommand>(c => c.EntityId == resourceId),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteOwner_ReturnsDispatcherResult()
    {
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteIncomeOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.DeleteOwner(Guid.NewGuid());

        Assert.True(result.IsSuccess);
    }

    #endregion
}
