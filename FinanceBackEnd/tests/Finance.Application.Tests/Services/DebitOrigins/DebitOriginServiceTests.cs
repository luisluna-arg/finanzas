using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.DebitOrigins;
using Finance.Application.Services;
using Finance.Application.Services.DebitOrigins;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Services.DebitOrigins;

public class DebitOriginServiceTests : IDisposable
{
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly FinanceDbContext _dbContext;
    private readonly DebitOriginService _sut;

    public DebitOriginServiceTests()
    {
        _dispatcher = new Mock<IDispatcher<FinanceDispatchContext>>();

        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
        _sut = new DebitOriginService(_dispatcher.Object, _dbContext);
    }

    public void Dispose() => _dbContext.Dispose();

    #region Create

    [Fact]
    public async Task Create_WhenBothDispatchesSucceed_ReturnsSuccess()
    {
        var origin = new DebitOrigin { Id = Guid.NewGuid(), Name = "Netflix" };
        var request = new CreateDebitOriginRequest(Guid.NewGuid(), "Netflix", false);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<DebitOrigin>>(It.IsAny<CreateDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitOrigin>.Success(origin));
        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<DebitOriginPermissions>>(It.IsAny<CreateDebitOriginPermissionsCommand>(), It.IsAny<HttpRequest?>()))
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
            .Setup(d => d.DispatchAsync<DataResult<DebitOrigin>>(It.IsAny<CreateDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitOrigin>.Success(new DebitOrigin { Id = Guid.NewGuid() }));
        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<DebitOriginPermissions>>(It.IsAny<CreateDebitOriginPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitOriginPermissions>.Success(new DebitOriginPermissions()));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<DebitOrigin>>(
            It.Is<CreateDebitOriginCommand>(c =>
                c.AppModuleId == appModuleId &&
                c.Name == "Spotify" &&
                c.Deactivated == false),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_DispatchesPermissionsCommandWithOwnerLevel()
    {
        var originId = Guid.NewGuid();
        var request = new CreateDebitOriginRequest(Guid.NewGuid(), "Netflix", false);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<DebitOrigin>>(It.IsAny<CreateDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitOrigin>.Success(new DebitOrigin { Id = originId }));
        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<DebitOriginPermissions>>(It.IsAny<CreateDebitOriginPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitOriginPermissions>.Success(new DebitOriginPermissions()));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<DebitOriginPermissions>>(
            It.Is<CreateDebitOriginPermissionsCommand>(c =>
                c.ResourceId == originId &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenCreateCommandFails_ReturnsFailure()
    {
        var request = new CreateDebitOriginRequest(Guid.NewGuid(), "Netflix", false);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<DebitOrigin>>(It.IsAny<CreateDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
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
            .Setup(d => d.DispatchAsync<DataResult<DebitOrigin>>(It.IsAny<CreateDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitOrigin>.Failure("create error"));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<DebitOriginPermissions>>(
            It.IsAny<CreateDebitOriginPermissionsCommand>(), It.IsAny<HttpRequest?>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new CreateDebitOriginRequest(Guid.NewGuid(), "Netflix", false);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<DebitOrigin>>(It.IsAny<CreateDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
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

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var request = new DeleteDebitOriginRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteDebitOriginOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Delete(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_DeletesOwnerForEachId()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var request = new DeleteDebitOriginRequest(ids);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteDebitOriginOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteDebitOriginOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Exactly(ids.Length));
    }

    [Fact]
    public async Task Delete_WhenDeleteDispatchFails_ReturnsFailure()
    {
        var request = new DeleteDebitOriginRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Failure("delete error"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("delete error", result.ErrorMessage);
    }

    [Fact]
    public async Task Delete_WhenDeleteDispatchFails_DoesNotDispatchOwnerDelete()
    {
        var request = new DeleteDebitOriginRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Failure("delete error"));

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteDebitOriginOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Never);
    }

    #endregion

    #region SetOwner

    [Fact]
    public async Task SetOwner_DispatchesCreatePermissionsCommandWithOwnerLevel()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<DebitOriginPermissions>>(It.IsAny<CreateDebitOriginPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitOriginPermissions>.Success(new DebitOriginPermissions()));

        await _sut.SetOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<DebitOriginPermissions>>(
            It.Is<CreateDebitOriginPermissionsCommand>(c =>
                c.ResourceId == resourceId &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    #endregion

    #region DeleteOwner

    [Fact]
    public async Task DeleteOwner_DispatchesDeleteOwnerCommandWithCorrectId()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteDebitOriginOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.DeleteOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteDebitOriginOwnerCommand>(c => c.EntityId == resourceId),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    #endregion

    #region Activate

    [Fact]
    public async Task Activate_DispatchesActivateCommandWithCorrectIds()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var request = new ActivateDebitOriginRequest(ids);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<ActivateDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Activate(request);

        Assert.True(result.IsSuccess);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<ActivateDebitOriginCommand>(c => c.Ids.SequenceEqual(ids)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    #endregion

    #region Deactivate

    [Fact]
    public async Task Deactivate_DispatchesDeactivateCommandWithCorrectIds()
    {
        var ids = new[] { Guid.NewGuid() };
        var request = new DeactivateDebitOriginRequest(ids);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeactivateDebitOriginCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Deactivate(request);

        Assert.True(result.IsSuccess);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeactivateDebitOriginCommand>(c => c.Ids.SequenceEqual(ids)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    #endregion
}
