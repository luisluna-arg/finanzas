using CQRSDispatch;
using Finance.Application.Commands.DebitOrigins;
using Finance.Application.Services.DebitOrigins;
using Finance.Domain.Models.Debits;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.DebitOrigins;

public partial class DebitOriginServiceTests : IDisposable
{
    [Fact]
    public async Task Update_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var origin = new DebitOrigin { Id = Guid.NewGuid() };
        var request = new UpdateDebitOriginRequest(origin.Id, Guid.NewGuid(), "Netflix", false);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<DebitOrigin>>(It.IsAny<UpdateDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitOrigin>.Success(origin));

        var result = await _sut.Update(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(origin, result.Data);
    }

    [Fact]
    public async Task Update_DispatchesCommandWithCorrectProperties()
    {
        var id = Guid.NewGuid();
        var appModuleId = Guid.NewGuid();
        var request = new UpdateDebitOriginRequest(id, appModuleId, "Spotify", true);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<DebitOrigin>>(It.IsAny<UpdateDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitOrigin>.Success(new DebitOrigin()));

        await _sut.Update(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<DebitOrigin>>(
            It.Is<UpdateDebitOriginCommand>(c =>
                c.Id == id &&
                c.AppModuleId == appModuleId &&
                c.Name == "Spotify" &&
                c.Deactivated == true),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }
}
