using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands;
using Finance.Application.Commands.Subscriptions;
using Finance.Application.Services;
using Finance.Application.Services.Subscriptions;
using Finance.Domain.Enums;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Services.Subscriptions;

public class SubscriptionServiceTests : IDisposable
{
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly FinanceDbContext _dbContext;
    private readonly SubscriptionService _sut;

    public SubscriptionServiceTests()
    {
        _dispatcher = new Mock<IDispatcher<FinanceDispatchContext>>();

        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
        _sut = new SubscriptionService(_dispatcher.Object, _dbContext);
    }

    public void Dispose() => _dbContext.Dispose();

    #region Create

    [Fact]
    public async Task Create_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var subscription = new Subscription { Id = Guid.NewGuid(), Name = "Netflix" };
        var request = new CreateSubscriptionRequest(Guid.NewGuid(), "Netflix", 9.99m, FrequencyEnum.Monthly);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Subscription>>(It.IsAny<CreateSubscriptionCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Subscription>.Success(subscription));

        var result = await _sut.Create(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(subscription, result.Data);
    }

    [Fact]
    public async Task Create_WhenDispatchSucceeds_DispatchesCommandWithCorrectProperties()
    {
        var currencyId = Guid.NewGuid();
        var request = new CreateSubscriptionRequest(currencyId, "Netflix", 9.99m, FrequencyEnum.Annual);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Subscription>>(It.IsAny<CreateSubscriptionCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Subscription>.Success(new Subscription()));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<Subscription>>(
            It.Is<CreateSubscriptionCommand>(c =>
                c.Name == request.Name &&
                c.Price == request.Price &&
                c.CurrencyId == currencyId &&
                c.Frequency == request.Frequency),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenDispatchFails_ReturnsFailure()
    {
        var request = new CreateSubscriptionRequest(Guid.NewGuid(), "Netflix", 9.99m, FrequencyEnum.Monthly);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Subscription>>(It.IsAny<CreateSubscriptionCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Subscription>.Failure("dispatch error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("dispatch error", result.ErrorMessage);
    }

    [Fact]
    public async Task Create_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new CreateSubscriptionRequest(Guid.NewGuid(), "Netflix", 9.99m, FrequencyEnum.Monthly);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Subscription>>(It.IsAny<CreateSubscriptionCommand>(), It.IsAny<HttpRequest?>()))
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
        var subscription = new Subscription { Id = Guid.NewGuid(), Name = "Netflix" };
        var request = new UpdateSubscriptionRequest(subscription.Id, Guid.NewGuid(), "Netflix", 9.99m, FrequencyEnum.Monthly);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Subscription>>(It.IsAny<UpdateSubscriptionCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Subscription>.Success(subscription));

        var result = await _sut.Update(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(subscription, result.Data);
    }

    [Fact]
    public async Task Update_DispatchesCommandWithCorrectProperties()
    {
        var subscriptionId = Guid.NewGuid();
        var currencyId = Guid.NewGuid();
        var request = new UpdateSubscriptionRequest(subscriptionId, currencyId, "Spotify", 4.99m, FrequencyEnum.Annual);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Subscription>>(It.IsAny<UpdateSubscriptionCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Subscription>.Success(new Subscription()));

        await _sut.Update(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<Subscription>>(
            It.Is<UpdateSubscriptionCommand>(c =>
                c.Id == subscriptionId &&
                c.Name == request.Name &&
                c.Price == request.Price &&
                c.CurrencyId == currencyId &&
                c.Frequency == request.Frequency),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenDispatchFails_ReturnsFailure()
    {
        var request = new UpdateSubscriptionRequest(Guid.NewGuid(), Guid.NewGuid(), "Netflix", 9.99m, FrequencyEnum.Monthly);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Subscription>>(It.IsAny<UpdateSubscriptionCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Subscription>.Failure("not found"));

        var result = await _sut.Update(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("not found", result.ErrorMessage);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var request = new DeleteSubscriptionRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteSubscriptionCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteSubscriptionOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Delete(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_DeletesOwnerForEachId()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var request = new DeleteSubscriptionRequest([id1, id2]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteSubscriptionCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteSubscriptionOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteSubscriptionOwnerCommand>(c => c.EntityId == id1),
            It.IsAny<HttpRequest?>()),
            Times.Once);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteSubscriptionOwnerCommand>(c => c.EntityId == id2),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Delete_WhenDeleteCommandFails_ReturnsFailureAndSkipsOwnerDeletion()
    {
        var request = new DeleteSubscriptionRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteSubscriptionCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Failure("delete failed"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("delete failed", result.ErrorMessage);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteSubscriptionOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new DeleteSubscriptionRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteSubscriptionCommand>(), It.IsAny<HttpRequest?>()))
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
            .Setup(d => d.DispatchAsync<DataResult<SubscriptionPermissions>>(It.IsAny<CreateSubscriptionPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<SubscriptionPermissions>.Success(new SubscriptionPermissions()));

        await _sut.SetOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<SubscriptionPermissions>>(
            It.Is<CreateSubscriptionPermissionsCommand>(c =>
                c.ResourceId == resourceId &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task SetOwner_ReturnsDispatcherResult()
    {
        var permissions = new SubscriptionPermissions();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<SubscriptionPermissions>>(It.IsAny<CreateSubscriptionPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<SubscriptionPermissions>.Success(permissions));

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
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteSubscriptionOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.DeleteOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteSubscriptionOwnerCommand>(c => c.EntityId == resourceId),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteOwner_ReturnsDispatcherResult()
    {
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteSubscriptionOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.DeleteOwner(Guid.NewGuid());

        Assert.True(result.IsSuccess);
    }

    #endregion
}
