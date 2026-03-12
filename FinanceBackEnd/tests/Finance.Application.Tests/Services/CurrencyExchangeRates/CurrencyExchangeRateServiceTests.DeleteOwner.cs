using CQRSDispatch;
using Finance.Application.Commands.CurrencyExchangeRates;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.CurrencyExchangeRates;

public partial class CurrencyExchangeRateServiceTests : IDisposable
{
    [Fact]
    public async Task DeleteOwner_DispatchesDeleteOwnerCommand()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteCurrencyExchangeRateOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.DeleteOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteCurrencyExchangeRateOwnerCommand>(c => c.EntityId == resourceId),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }
}
