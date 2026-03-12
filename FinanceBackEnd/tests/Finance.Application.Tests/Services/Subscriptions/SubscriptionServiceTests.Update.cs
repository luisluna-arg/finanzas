using CQRSDispatch;
using Finance.Application.Commands.Subscriptions;
using Finance.Application.Services.Subscriptions;
using Finance.Domain.Enums;
using Finance.Domain.Models.Subscriptions;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Subscriptions;

public partial class SubscriptionServiceTests : IDisposable
{
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
}
