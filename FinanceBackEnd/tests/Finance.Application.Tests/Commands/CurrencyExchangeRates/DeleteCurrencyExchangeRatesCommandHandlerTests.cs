using Finance.Application.Commands.CurrencyExchangeRates;
using Finance.Application.Repositories;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Tests.Commands.CurrencyExchangeRates;

public sealed class DeleteCurrencyExchangeRatesCommandHandlerTests
{
    private readonly Mock<IRepository<CurrencyExchangeRate, Guid>> _repository = new();

    [Fact]
    public async Task Delete_HappyPath_CallsServiceDeleteAndReturnsSuccess()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var command = new DeleteCurrencyExchangeRatesCommand() { Ids = ids };

        _repository.Setup(s => s.DeleteAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>(),
            false))
            .Returns(Task.CompletedTask);

        var handler = new DeleteCurrencyExchangeRatesCommandHandler(_repository.Object);
        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        _repository.Verify(s => s.DeleteAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>(),
            false),
            Times.Exactly(ids.Length));
        _repository.Verify(s => s.PersistAsync(
            It.IsAny<CancellationToken>()),
            Times.Once());
    }
}
