using CQRSDispatch;
using Finance.Application.Commands.DebitOrigins;
using Finance.Application.Services.DebitOrigins;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.DebitOrigins;

public partial class DebitOriginServiceTests : IDisposable
{
    [Fact]
    public async Task Create_WhenBothDispatchesSucceed_ReturnsSuccess()
    {
        var origin = new DebitOrigin { Id = Guid.NewGuid(), Name = "Netflix" };
        var request = new CreateDebitOriginRequest(Guid.NewGuid(), "Netflix", false);

        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitOrigin>.Success(origin));
        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateDebitOriginPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitOriginPermissions>.Success(new DebitOriginPermissions()));

        var result = await _sut.Create(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(origin, result.Data);
    }

    [Fact]
    public async Task Create_DispatchesCreateCommandWithCorrectProperties()
    {
        var appModuleId = Guid.NewGuid();
        var request = new CreateDebitOriginRequest(appModuleId, "Spotify", false);

        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitOrigin>.Success(new DebitOrigin { Id = Guid.NewGuid() }));
        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateDebitOriginPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitOriginPermissions>.Success(new DebitOriginPermissions()));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync(
            It.Is<CreateDebitOriginCommand>(c =>
                c.AppModuleId == appModuleId &&
                c.Name == "Spotify" &&
                c.Deactivated == false),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenCreateCommandFails_ReturnsFailure()
    {
        var request = new CreateDebitOriginRequest(Guid.NewGuid(), "Netflix", false);

        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitOrigin>.Failure("create error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("create error", result.ErrorMessage);
    }

    [Fact]
    public async Task Create_WhenCreateCommandFails_DoesNotDispatchPermissions()
    {
        var request = new CreateDebitOriginRequest(Guid.NewGuid(), "Netflix", false);

        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitOrigin>.Failure("create error"));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync(
            It.IsAny<CreateDebitOriginPermissionsCommand>(), It.IsAny<HttpRequest?>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new CreateDebitOriginRequest(Guid.NewGuid(), "Netflix", false);

        _dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<CreateDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .Throws(new Exception("unexpected error"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected error", result.ErrorMessage);
    }
}