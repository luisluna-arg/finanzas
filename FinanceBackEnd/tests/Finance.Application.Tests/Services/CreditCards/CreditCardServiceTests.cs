using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.CreditCards;
using Finance.Application.Services;
using Finance.Application.Services.CreditCards;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using CreateCreditCardCommand = Finance.Application.Legacy.Commands.CreditCards.CreateCreditCardCommand;
using DeleteCreditCardCommand = Finance.Application.Legacy.Commands.CreditCards.DeleteCreditCardCommand;
using UpdateCreditCardCommand = Finance.Application.Legacy.Commands.CreditCards.UpdateCreditCardCommand;

namespace Finance.Application.Tests.Services.CreditCards;

public class CreditCardServiceTests : IDisposable
{
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly FinanceDbContext _dbContext;
    private readonly CreditCardService _sut;

    public CreditCardServiceTests()
    {
        _dispatcher = new Mock<IDispatcher<FinanceDispatchContext>>();

        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
        _sut = new CreditCardService(_dispatcher.Object, _dbContext);
    }

    public void Dispose() => _dbContext.Dispose();

    #region Create

    [Fact]
    public async Task Create_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var creditCard = new CreditCard { Id = Guid.NewGuid() };
        var request = new CreateCreditCardRequest(Guid.NewGuid(), "My Card", false);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CreditCard>>(It.IsAny<CreateCreditCardCommand>()))
            .ReturnsAsync(DataResult<CreditCard>.Success(creditCard));
        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CreditCardPermissions>>(It.IsAny<CreateCreditCardPermissionsCommand>(), It.IsAny<HttpRequest?>()))
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
            .Setup(d => d.DispatchAsync<DataResult<CreditCard>>(It.IsAny<CreateCreditCardCommand>()))
            .ReturnsAsync(DataResult<CreditCard>.Success(new CreditCard()));
        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CreditCardPermissions>>(It.IsAny<CreateCreditCardPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<CreditCardPermissions>.Success(new CreditCardPermissions()));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<CreditCard>>(
            It.Is<CreateCreditCardCommand>(c =>
                c.BankId == bankId &&
                c.Name == request.Name &&
                c.Deactivated == request.Deactivated)),
            Times.Once);
    }

    [Fact]
    public async Task Create_DispatchesPermissionsCommandWithOwnerLevel()
    {
        var creditCard = new CreditCard { Id = Guid.NewGuid() };
        var request = new CreateCreditCardRequest(Guid.NewGuid(), "My Card", false);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CreditCard>>(It.IsAny<CreateCreditCardCommand>()))
            .ReturnsAsync(DataResult<CreditCard>.Success(creditCard));
        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CreditCardPermissions>>(It.IsAny<CreateCreditCardPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<CreditCardPermissions>.Success(new CreditCardPermissions()));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<CreditCardPermissions>>(
            It.Is<CreateCreditCardPermissionsCommand>(c =>
                c.ResourceId == creditCard.Id &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenDispatchFails_ReturnsFailure()
    {
        var request = new CreateCreditCardRequest(Guid.NewGuid(), "My Card", false);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CreditCard>>(It.IsAny<CreateCreditCardCommand>()))
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
            .Setup(d => d.DispatchAsync<DataResult<CreditCard>>(It.IsAny<CreateCreditCardCommand>()))
            .ReturnsAsync(DataResult<CreditCard>.Failure("dispatch error"));

        await _sut.Create(request);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<CreditCardPermissions>>(
            It.IsAny<CreateCreditCardPermissionsCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenDispatchThrows_ReturnsFailure()
    {
        var request = new CreateCreditCardRequest(Guid.NewGuid(), "My Card", false);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CreditCard>>(It.IsAny<CreateCreditCardCommand>()))
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

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var ids = new[] { Guid.NewGuid() };
        var request = new DeleteCreditCardRequest(ids);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteCreditCardCommand>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteCreditCardOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.Delete(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_DeletesOwnerForEachId()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var request = new DeleteCreditCardRequest(ids);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteCreditCardCommand>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteCreditCardOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteCreditCardOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Exactly(ids.Length));
    }

    [Fact]
    public async Task Delete_DeletesOwnerWithCorrectEntityId()
    {
        var id = Guid.NewGuid();
        var request = new DeleteCreditCardRequest([id]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteCreditCardCommand>()))
            .ReturnsAsync(CommandResult.Success());
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteCreditCardOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteCreditCardOwnerCommand>(c => c.EntityId == id),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task Delete_WhenDeleteDispatchFails_ReturnsFailure()
    {
        var request = new DeleteCreditCardRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteCreditCardCommand>()))
            .ReturnsAsync(CommandResult.Failure("delete error"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("delete error", result.ErrorMessage);
    }

    [Fact]
    public async Task Delete_WhenDeleteDispatchFails_DoesNotDispatchOwnerDelete()
    {
        var request = new DeleteCreditCardRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteCreditCardCommand>()))
            .ReturnsAsync(CommandResult.Failure("delete error"));

        await _sut.Delete(request);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.IsAny<DeleteCreditCardOwnerCommand>(),
            It.IsAny<HttpRequest?>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_WhenThrows_ReturnsFailure()
    {
        var request = new DeleteCreditCardRequest([Guid.NewGuid()]);

        _dispatcher
            .Setup(d => d.DispatchCommandAsync(It.IsAny<DeleteCreditCardCommand>()))
            .Throws(new Exception("unexpected"));

        var result = await _sut.Delete(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("unexpected", result.ErrorMessage);
    }

    #endregion

    #region SetOwner

    [Fact]
    public async Task SetOwner_DispatchesCreatePermissionsCommandWithOwnerLevel()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CreditCardPermissions>>(It.IsAny<CreateCreditCardPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<CreditCardPermissions>.Success(new CreditCardPermissions()));

        await _sut.SetOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<DataResult<CreditCardPermissions>>(
            It.Is<CreateCreditCardPermissionsCommand>(c =>
                c.ResourceId == resourceId &&
                c.PermissionLevels.Contains(PermissionLevelEnum.Owner)),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task SetOwner_ReturnsDispatchResult()
    {
        var permissions = new CreditCardPermissions();

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<CreditCardPermissions>>(It.IsAny<CreateCreditCardPermissionsCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<CreditCardPermissions>.Success(permissions));

        var result = await _sut.SetOwner(Guid.NewGuid());

        Assert.True(result.IsSuccess);
        Assert.Equal(permissions, result.Data);
    }

    #endregion

    #region DeleteOwner

    [Fact]
    public async Task DeleteOwner_DispatchesDeleteOwnerCommandWithCorrectId()
    {
        var resourceId = Guid.NewGuid();

        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteCreditCardOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        await _sut.DeleteOwner(resourceId);

        _dispatcher.Verify(d => d.DispatchAsync<CommandResult>(
            It.Is<DeleteCreditCardOwnerCommand>(c => c.EntityId == resourceId),
            It.IsAny<HttpRequest?>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteOwner_ReturnsDispatchResult()
    {
        _dispatcher
            .Setup(d => d.DispatchAsync<CommandResult>(It.IsAny<DeleteCreditCardOwnerCommand>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(CommandResult.Success());

        var result = await _sut.DeleteOwner(Guid.NewGuid());

        Assert.True(result.IsSuccess);
    }

    #endregion
}
