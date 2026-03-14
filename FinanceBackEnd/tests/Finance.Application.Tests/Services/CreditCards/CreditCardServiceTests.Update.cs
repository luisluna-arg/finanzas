using CQRSDispatch;
using Finance.Application.Commands.CreditCards;
using Finance.Application.Services.CreditCards;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Tests.Services.CreditCards;

public partial class CreditCardServiceTests : IDisposable
{
    [Fact]
    public async Task Update_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var creditCard = new CreditCard { Id = Guid.NewGuid() };
        var request = new UpdateCreditCardRequest(creditCard.Id, Guid.NewGuid(), "Updated Card", false);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CreditCard>>(It.IsAny<UpdateCreditCardCommand>()))
            .ReturnsAsync(DataResult<CreditCard>.Success(creditCard));

        var result = await _sut.Update(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(creditCard, result.Data);
    }

    [Fact]
    public async Task Update_DispatchesCommandWithCorrectProperties()
    {
        var id = Guid.NewGuid();
        var bankId = Guid.NewGuid();
        var request = new UpdateCreditCardRequest(id, bankId, "Mastercard Platinum", true);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CreditCard>>(It.IsAny<UpdateCreditCardCommand>()))
            .ReturnsAsync(DataResult<CreditCard>.Success(new CreditCard()));

        await _sut.Update(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<CreditCard>>(
            It.Is<UpdateCreditCardCommand>(c =>
                c.Id == id &&
                c.BankId == bankId &&
                c.Name == request.Name &&
                c.Deactivated == request.Deactivated)),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenDispatchFails_ReturnsFailure()
    {
        var request = new UpdateCreditCardRequest(Guid.NewGuid(), Guid.NewGuid(), "Card", false);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CreditCard>>(It.IsAny<UpdateCreditCardCommand>()))
            .ReturnsAsync(DataResult<CreditCard>.Failure("not found"));

        var result = await _sut.Update(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("not found", result.ErrorMessage);
    }
}