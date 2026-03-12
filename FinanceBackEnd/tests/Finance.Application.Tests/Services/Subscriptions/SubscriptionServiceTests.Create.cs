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
}
