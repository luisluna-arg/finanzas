using CQRSDispatch;
using Finance.Application.Commands.Debits;
using Finance.Application.Services.Debits;
using Finance.Domain.Enums;
using Finance.Domain.Models.Debits;
using Finance.Domain.SpecialTypes;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.Debits;

public partial class DebitServiceTests : IDisposable
{
    [Fact]
    public async Task Create_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var debit = new Debit { Id = Guid.NewGuid() };
        var request = new CreateDebitRequest(Guid.NewGuid(), "Rent", new Money(500m), false, FrequencyEnum.Monthly);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Debit>>(It.IsAny<CreateDebitCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Debit>.Success(debit));

        var result = await _sut.Create(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(debit, result.Data);
    }

    [Fact]
    public async Task Create_DispatchesCommandWithCorrectProperties()
    {
        var appModuleId = Guid.NewGuid();
        var request = new CreateDebitRequest(appModuleId, "Internet", new Money(30m), false, FrequencyEnum.Annual);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Debit>>(It.IsAny<CreateDebitCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Debit>.Success(new Debit()));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<Debit>>(
            It.Is<CreateDebitCommand>(c =>
                c.AppModuleId == appModuleId &&
                c.Origin == request.Origin &&
                c.Amount == request.Amount &&
                c.Deactivated == request.Deactivated &&
                c.Frequency == request.Frequency),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenDispatchFails_ReturnsFailure()
    {
        var request = new CreateDebitRequest(Guid.NewGuid(), "Rent", new Money(500m), false, FrequencyEnum.Monthly);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Debit>>(It.IsAny<CreateDebitCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Debit>.Failure("dispatch error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("dispatch error", result.ErrorMessage);
    }

    [Fact]
    public async Task Create_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new CreateDebitRequest(Guid.NewGuid(), "Rent", new Money(500m), false, FrequencyEnum.Monthly);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Debit>>(It.IsAny<CreateDebitCommand>(), It.IsAny<HttpRequest?>()))
            .Throws(new Exception("unexpected error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected error", result.ErrorMessage);
    }
}
