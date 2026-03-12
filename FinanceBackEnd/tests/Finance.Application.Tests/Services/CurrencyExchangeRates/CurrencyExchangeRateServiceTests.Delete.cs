using CQRSDispatch;
using Finance.Application.Commands.CurrencyExchangeRates;
using Finance.Application.Services.CurrencyExchangeRates;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.CurrencyExchangeRates;

public partial class CurrencyExchangeRateServiceTests : IDisposable
{
    [Fact]
    public async Task Delete_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var request = new DeleteCurrencyExchangeRateRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteCurrencyExchangeRatesCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteCurrencyExchangeRateOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Delete(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_DeletesOwnerForEachId()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var request = new DeleteCurrencyExchangeRateRequest([id1, id2]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteCurrencyExchangeRatesCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteCurrencyExchangeRateOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteCurrencyExchangeRateOwnerCommand>(c => c.EntityId == id1),
            It.IsAny<HttpRequest?>()),
            Times.Once);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteCurrencyExchangeRateOwnerCommand>(c => c.EntityId == id2),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Delete_WhenDispatchFails_ReturnsFailure()
    {
        var request = new DeleteCurrencyExchangeRateRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteCurrencyExchangeRatesCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Failure("delete failed"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("Type CurrencyExchangeRate delete operation failed", result.ErrorMessage);
    }

    [Fact]
    public async Task Delete_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new DeleteCurrencyExchangeRateRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteCurrencyExchangeRatesCommand>(), It.IsAny<HttpRequest?>()))
            .Throws(new Exception("unexpected error"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected error", result.ErrorMessage);
    }
}
