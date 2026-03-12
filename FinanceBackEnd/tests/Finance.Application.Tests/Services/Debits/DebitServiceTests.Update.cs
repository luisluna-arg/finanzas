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
    public async Task Update_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var debit = new Debit { Id = Guid.NewGuid() };
        var request = new UpdateDebitRequest(debit.Id, Guid.NewGuid(), "Rent", new Money(500m), false, FrequencyEnum.Monthly);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Debit>>(It.IsAny<UpdateDebitCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Debit>.Success(debit));

        var result = await _sut.Update(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(debit, result.Data);
    }

    [Fact]
    public async Task Update_DispatchesCommandWithCorrectProperties()
    {
        var id = Guid.NewGuid();
        var appModuleId = Guid.NewGuid();
        var request = new UpdateDebitRequest(id, appModuleId, "Gym", new Money(25m), false, FrequencyEnum.Annual);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<Debit>>(It.IsAny<UpdateDebitCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<Debit>.Success(new Debit()));

        await _sut.Update(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<Debit>>(
            It.Is<UpdateDebitCommand>(c =>
                c.Id == id &&
                c.AppModuleId == appModuleId &&
                c.Origin == request.Origin &&
                c.Amount == request.Amount &&
                c.Frequency == request.Frequency),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }
}
