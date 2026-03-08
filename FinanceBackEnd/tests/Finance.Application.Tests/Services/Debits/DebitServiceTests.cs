using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Debits;
using Finance.Application.Services;
using Finance.Application.Services.Debits;
using Finance.Domain.Enums;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Services.Debits;

public class DebitServiceTests : IDisposable
{
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly FinanceDbContext _dbContext;
    private readonly DebitService _sut;

    public DebitServiceTests()
    {
        _dispatcher = new Mock<IDispatcher<FinanceDispatchContext>>();

        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
        _sut = new DebitService(_dispatcher.Object, _dbContext);
    }

    public void Dispose() => _dbContext.Dispose();

    #region Create

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

    #endregion

    #region Update

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

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var request = new DeleteDebitRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteDebitCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteDebitOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Delete(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_DeletesOwnerForEachId()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var request = new DeleteDebitRequest(ids);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteDebitCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteDebitOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteDebitOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Exactly(ids.Length));
    }

    [Fact]
    public async Task Delete_WhenDeleteDispatchFails_ReturnsFailure()
    {
        var request = new DeleteDebitRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteDebitCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Failure("delete error"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("delete error", result.ErrorMessage);
    }

    [Fact]
    public async Task Delete_WhenDeleteDispatchFails_DoesNotDispatchOwnerDelete()
    {
        var request = new DeleteDebitRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteDebitCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Failure("delete error"));

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteDebitOwnerCommand>(),
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
            .Setup(d => d.DispatchAsync<DataResult<DebitPermissions>>(It.IsAny<CreateDebitPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<DebitPermissions>.Success(new DebitPermissions()));

        await _sut.SetOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<DebitPermissions>>(
            It.Is<CreateDebitPermissionsCommand>(c =>
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
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteDebitOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.DeleteOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteDebitOwnerCommand>(c => c.EntityId == resourceId),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    #endregion

    #region Activate

    [Fact]
    public async Task Activate_DispatchesActivateCommandWithCorrectIds()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var request = new ActivateDebitRequest(ids);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<ActivateDebitCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Activate(request);

        Assert.True(result.IsSuccess);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<ActivateDebitCommand>(c => c.Ids.SequenceEqual(ids)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    #endregion

    #region Deactivate

    [Fact]
    public async Task Deactivate_DispatchesDeactivateCommandWithCorrectIds()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var request = new DeactivateDebitRequest(ids);

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeactivateDebitCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Deactivate(request);

        Assert.True(result.IsSuccess);
        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeactivateDebitCommand>(c => c.Ids.SequenceEqual(ids)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    #endregion
}
