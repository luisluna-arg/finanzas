using Finance.Application.Commands.CurrencyExchangeRates;
using Finance.Application.Services;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Tests.Commands.CurrencyExchangeRates;

public class DeleteCurrencyExchangeRatesCommandHandlerTests
{
    private readonly Mock<IEntityService<CurrencyExchangeRate, Guid>> _service;

    public DeleteCurrencyExchangeRatesCommandHandlerTests()
    {
        _service = new Mock<IEntityService<CurrencyExchangeRate, Guid>>();
    }

    [Fact]
    public async Task Delete_HappyPath_CallsServiceDeleteAndReturnsSuccess()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var command = new DeleteCurrencyExchangeRatesCommand(ids);

        _service.Setup(s => s.DeleteAsync(It.IsAny<ICollection<Guid>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new DeleteCurrencyExchangeRatesCommandHandler(_service.Object);
        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        _service.Verify(s => s.DeleteAsync(
            It.Is<ICollection<Guid>>(c => c.SequenceEqual(ids)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
