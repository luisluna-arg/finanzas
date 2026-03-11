using Finance.Application.Commands.DebitOrigins;
using Finance.Application.Repositories;
using Finance.Domain.Models.Debits;
using FluentValidation;

namespace Finance.Application.Tests.Commands.DebitOrigins;

public sealed class DeleteDebitOriginCommandHandlerTests
{
    private readonly Mock<IRepository<DebitOrigin, Guid>> _entityService = new();

    [Fact]
    public async Task Delete_ValidIds_DeletesAndReturnsSuccess()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        _entityService.Setup(s => s.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
            .Returns(Task.CompletedTask);

        var handler = new DeleteDebitOriginCommandHandler(_entityService.Object);
        var result = await handler.ExecuteAsync(new DeleteDebitOriginCommand { Ids = ids }, default);

        Assert.True(result.IsSuccess);
        _entityService.Verify(s => s.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false), Times.Exactly(ids.Length));
        _entityService.Verify(s => s.PersistAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Delete_EmptyIds_ThrowsValidationException()
    {
        var handler = new DeleteDebitOriginCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ExecuteAsync(new DeleteDebitOriginCommand() { Ids = [] }, default));
    }

    [Fact]
    public async Task Delete_EmptyGuidInIds_ThrowsValidationException()
    {
        var handler = new DeleteDebitOriginCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ExecuteAsync(new DeleteDebitOriginCommand { Ids = [Guid.Empty] }, default));
    }
}
