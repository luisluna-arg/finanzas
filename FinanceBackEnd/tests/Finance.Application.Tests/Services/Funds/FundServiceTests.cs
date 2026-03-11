using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands;
using Finance.Application.Commands.Funds;
using Finance.Application.Services;
using Finance.Application.Services.Funds;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Funds;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Services.Funds;

public class FundServiceTests : IDisposable
{
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly FinanceDbContext _dbContext;
    private readonly FundService _sut;

    public FundServiceTests()
    {
        _dispatcher = new Mock<IDispatcher<FinanceDispatchContext>>();

        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
        _sut = new FundService(_dispatcher.Object, _dbContext);
    }

    public void Dispose() => _dbContext.Dispose();

    #region Create

    [Fact]
    public async Task Create_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var fund = new Fund { Id = Guid.NewGuid() };
        var request = new CreateFundRequest(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(100m), true);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Fund>>(It.IsAny<CreateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Fund>.Success(fund));

        var result = await _sut.Create(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(fund, result.Data);
    }

    [Fact]
    public async Task Create_WhenDispatchSucceeds_DispatchesCommandWithCorrectProperties()
    {
        var bankId = Guid.NewGuid();
        var currencyId = Guid.NewGuid();
        var timeStamp = DateTime.UtcNow;
        var amount = new Money(250m);
        var request = new CreateFundRequest(bankId, currencyId, timeStamp, amount, false);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Fund>>(It.IsAny<CreateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Fund>.Success(new Fund()));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<Fund>>(
            It.Is<CreateFundCommand>(c =>
                c.BankId == bankId &&
                c.CurrencyId == currencyId &&
                c.TimeStamp == timeStamp &&
                c.Amount == amount &&
                c.DailyUse == false),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenDispatchFails_ReturnsFailure()
    {
        var request = new CreateFundRequest(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(100m), true);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Fund>>(It.IsAny<CreateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Fund>.Failure("bank not found"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("bank not found", result.ErrorMessage);
    }

    [Fact]
    public async Task Create_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new CreateFundRequest(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(100m), true);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Fund>>(It.IsAny<CreateFundCommand>(), It.IsAny<HttpRequest?>()))
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
        var fund = new Fund { Id = Guid.NewGuid() };
        var request = new UpdateFundRequest(fund.Id, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(150m), true);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Fund>>(It.IsAny<UpdateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Fund>.Success(fund));

        var result = await _sut.Update(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(fund, result.Data);
    }

    [Fact]
    public async Task Update_DispatchesCommandWithCorrectProperties()
    {
        var fundId = Guid.NewGuid();
        var bankId = Guid.NewGuid();
        var currencyId = Guid.NewGuid();
        var timeStamp = DateTime.UtcNow;
        var amount = new Money(500m);
        var request = new UpdateFundRequest(fundId, bankId, currencyId, timeStamp, amount, true);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Fund>>(It.IsAny<UpdateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Fund>.Success(new Fund()));

        await _sut.Update(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<Fund>>(
            It.Is<UpdateFundCommand>(c =>
                c.Id == fundId &&
                c.BankId == bankId &&
                c.CurrencyId == currencyId &&
                c.TimeStamp == timeStamp &&
                c.Amount == amount &&
                c.DailyUse == true),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenDispatchFails_ReturnsFailure()
    {
        var request = new UpdateFundRequest(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new Money(100m), false);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Fund>>(It.IsAny<UpdateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Fund>.Failure("fund not found"));

        var result = await _sut.Update(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("fund not found", result.ErrorMessage);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var request = new DeleteFundRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteFundsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteFundOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Delete(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_DeletesOwnerForEachId()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var request = new DeleteFundRequest([id1, id2]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteFundsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteFundOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteFundOwnerCommand>(c => c.EntityId == id1),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Delete_WhenDeleteCommandFails_ReturnsFailureAndSkipsOwnerDeletion()
    {
        var request = new DeleteFundRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteFundsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Failure("delete failed"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("Type Fund delete operation failed", result.ErrorMessage);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteFundOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new DeleteFundRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteFundsCommand>(), It.IsAny<HttpRequest?>()))
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
            .Setup(d => d.DispatchAsync<DataResult<FundPermissions>>(It.IsAny<CreateFundPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<FundPermissions>.Success(new FundPermissions()));

        await _sut.SetOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<FundPermissions>>(
            It.Is<CreateFundPermissionsCommand>(c =>
                c.ResourceId == resourceId &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task SetOwner_ReturnsDispatcherResult()
    {
        var permissions = new FundPermissions();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<FundPermissions>>(It.IsAny<CreateFundPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<FundPermissions>.Success(permissions));

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
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteFundOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.DeleteOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteFundOwnerCommand>(c => c.EntityId == resourceId),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteOwner_ReturnsDispatcherResult()
    {
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteFundOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.DeleteOwner(Guid.NewGuid());

        Assert.True(result.IsSuccess);
    }

    #endregion

    #region Activate

    [Fact]
    public async Task Activate_DispatchesActivateFundCommandWithCorrectIds()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<ActivateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Activate(ids);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<ActivateFundCommand>(c => c.Ids.SequenceEqual(ids)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Activate_ReturnsDispatcherResult()
    {
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<ActivateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Activate([Guid.NewGuid()]);

        Assert.True(result.IsSuccess);
    }

    #endregion

    #region Deactivate

    [Fact]
    public async Task Deactivate_DispatchesDeactivateFundCommandWithCorrectIds()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeactivateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Deactivate(ids);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeactivateFundCommand>(c => c.Ids.SequenceEqual(ids)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Deactivate_ReturnsDispatcherResult()
    {
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeactivateFundCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Deactivate([Guid.NewGuid()]);

        Assert.True(result.IsSuccess);
    }

    #endregion
}
