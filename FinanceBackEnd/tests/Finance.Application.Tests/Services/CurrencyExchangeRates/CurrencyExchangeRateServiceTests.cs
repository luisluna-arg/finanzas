using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.CurrencyExchangeRates;
using Finance.Application.Services;
using Finance.Application.Services.CurrencyExchangeRates;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Services.CurrencyExchangeRates;

public class CurrencyExchangeRateServiceTests : IDisposable
{
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly FinanceDbContext _dbContext;
    private readonly CurrencyExchangeRateService _sut;

    public CurrencyExchangeRateServiceTests()
    {
        _dispatcher = new Mock<IDispatcher<FinanceDispatchContext>>();

        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
        _sut = new CurrencyExchangeRateService(_dispatcher.Object, _dbContext);
    }

    public void Dispose() => _dbContext.Dispose();

    #region Create

    [Fact]
    public async Task Create_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var rate = new CurrencyExchangeRate { Id = Guid.NewGuid() };
        var request = new CreateCurrencyExchangeRateRequest(Guid.NewGuid(), Guid.NewGuid(), 900m, 910m, DateTime.UtcNow);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(It.IsAny<CreateCurrencyExchangeRateCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<CurrencyExchangeRate>.Success(rate));

        var result = await _sut.Create(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(rate, result.Data);
    }

    [Fact]
    public async Task Create_DispatchesCommandWithCorrectProperties()
    {
        var baseCurrencyId = Guid.NewGuid();
        var quoteCurrencyId = Guid.NewGuid();
        var buyRate = 900m;
        var sellRate = 910m;
        var timeStamp = DateTime.UtcNow;
        var request = new CreateCurrencyExchangeRateRequest(baseCurrencyId, quoteCurrencyId, buyRate, sellRate, timeStamp);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(It.IsAny<CreateCurrencyExchangeRateCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<CurrencyExchangeRate>.Success(new CurrencyExchangeRate()));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(
            It.Is<CreateCurrencyExchangeRateCommand>(c =>
                c.BaseCurrencyId == baseCurrencyId &&
                c.QuoteCurrencyId == quoteCurrencyId &&
                c.BuyRate == buyRate &&
                c.SellRate == sellRate &&
                c.TimeStamp == timeStamp),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenDispatchFails_ReturnsFailure()
    {
        var request = new CreateCurrencyExchangeRateRequest(Guid.NewGuid(), Guid.NewGuid(), 900m, 910m, DateTime.UtcNow);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(It.IsAny<CreateCurrencyExchangeRateCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<CurrencyExchangeRate>.Failure("base currency not found"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("base currency not found", result.ErrorMessage);
    }

    [Fact]
    public async Task Create_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new CreateCurrencyExchangeRateRequest(Guid.NewGuid(), Guid.NewGuid(), 900m, 910m, DateTime.UtcNow);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(It.IsAny<CreateCurrencyExchangeRateCommand>(), It.IsAny<HttpRequest?>()))
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
        var rate = new CurrencyExchangeRate { Id = Guid.NewGuid() };
        var request = new UpdateCurrencyExchangeRateRequest(rate.Id, 900m, 910m);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(It.IsAny<UpdateCurrencyExchangeRateCommand>()))
            .ReturnsAsync(DataResult<CurrencyExchangeRate>.Success(rate));

        var result = await _sut.Update(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(rate, result.Data);
    }

    [Fact]
    public async Task Update_DispatchesCommandWithCorrectProperties()
    {
        var id = Guid.NewGuid();
        var buyRate = 900m;
        var sellRate = 910m;
        var request = new UpdateCurrencyExchangeRateRequest(id, buyRate, sellRate);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(It.IsAny<UpdateCurrencyExchangeRateCommand>()))
            .ReturnsAsync(DataResult<CurrencyExchangeRate>.Success(new CurrencyExchangeRate()));

        await _sut.Update(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(
            It.Is<UpdateCurrencyExchangeRateCommand>(c =>
                c.Id == id &&
                c.BuyRate == buyRate &&
                c.SellRate == sellRate)),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenDispatchFails_ReturnsFailure()
    {
        var request = new UpdateCurrencyExchangeRateRequest(Guid.NewGuid(), 900m, 910m);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CurrencyExchangeRate>>(It.IsAny<UpdateCurrencyExchangeRateCommand>()))
            .ReturnsAsync(DataResult<CurrencyExchangeRate>.Failure("rate not found"));

        var result = await _sut.Update(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("rate not found", result.ErrorMessage);
    }

    #endregion

    #region Delete

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

    #endregion

    #region SetOwner / DeleteOwner

    [Fact]
    public async Task SetOwner_DispatchesCreatePermissionsCommandWithOwner()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CurrencyExchangeRatePermissions>>(
                It.IsAny<CreateCurrencyExchangeRatePermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<CurrencyExchangeRatePermissions>.Success(new CurrencyExchangeRatePermissions()));

        await _sut.SetOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<CurrencyExchangeRatePermissions>>(
            It.Is<CreateCurrencyExchangeRatePermissionsCommand>(c =>
                c.ResourceId == resourceId &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

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

    #endregion
}
