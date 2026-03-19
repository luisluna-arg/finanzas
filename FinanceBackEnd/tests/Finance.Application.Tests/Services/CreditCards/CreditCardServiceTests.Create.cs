using CQRSDispatch;
using Finance.Application.Commands.CreditCards;
using Finance.Application.Services.CreditCards;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.CreditCards;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.CreditCards;

public partial class CreditCardServiceTests : IDisposable
{
    [Fact]
    public async Task Create_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var creditCard = new CreditCard { Id = Guid.NewGuid() };
        var request = new CreateCreditCardRequest(Guid.NewGuid(), "My Card", false);

        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateCreditCardCommand>()))
            .ReturnsAsync(DataResult<CreditCard>.Success(creditCard));
        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateCreditCardPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<CreditCardPermissions>.Success(new CreditCardPermissions()));

        var result = await _sut.Create(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(creditCard, result.Data);
    }

    [Fact]
    public async Task Create_DispatchesCommandWithCorrectProperties()
    {
        var bankId = Guid.NewGuid();
        var request = new CreateCreditCardRequest(bankId, "Visa Gold", true);

        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateCreditCardCommand>()))
            .ReturnsAsync(DataResult<CreditCard>.Success(new CreditCard()));
        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateCreditCardPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<CreditCardPermissions>.Success(new CreditCardPermissions()));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync(
            It.Is<CreateCreditCardCommand>(c =>
                c.BankId == bankId &&
                c.Name == request.Name &&
                c.Deactivated == request.Deactivated)),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenDispatchFails_ReturnsFailure()
    {
        var request = new CreateCreditCardRequest(Guid.NewGuid(), "My Card", false);

        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateCreditCardCommand>()))
            .ReturnsAsync(DataResult<CreditCard>.Failure("dispatch error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("dispatch error", result.ErrorMessage);
    }

    [Fact]
    public async Task Create_WhenDispatchFails_DoesNotDispatchPermissions()
    {
        var request = new CreateCreditCardRequest(Guid.NewGuid(), "My Card", false);

        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateCreditCardCommand>()))
            .ReturnsAsync(DataResult<CreditCard>.Failure("dispatch error"));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync(
            It.IsAny<CreateCreditCardPermissionsCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new CreateCreditCardRequest(Guid.NewGuid(), "My Card", false);

        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateCreditCardCommand>()))
            .Throws(new Exception("unexpected error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected error", result.ErrorMessage);
    }
}
