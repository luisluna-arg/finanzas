using Finance.Application.Commands.DebitOrigins;
using Finance.Application.Services;
using Finance.Domain.Models.Debits;
using FluentValidation;

namespace Finance.Application.Tests.Commands.DebitOrigins;

public class DeleteDebitOriginCommandHandlerTests
{
    private readonly Mock<IEntityService<DebitOrigin, Guid>> _entityService;

    public DeleteDebitOriginCommandHandlerTests()
    {
        _entityService = new Mock<IEntityService<DebitOrigin, Guid>>();
    }

    [Fact]
    public async Task Delete_ValidIds_DeletesAndReturnsSuccess()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        _entityService.Setup(s => s.DeleteAsync(It.IsAny<ICollection<Guid>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new DeleteDebitOriginCommandHandler(_entityService.Object);
        var result = await handler.ExecuteAsync(new DeleteDebitOriginCommand { Ids = ids }, default);

        Assert.True(result.IsSuccess);
        _entityService.Verify(s => s.DeleteAsync(ids, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_EmptyIds_ThrowsValidationException()
    {
        var handler = new DeleteDebitOriginCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ExecuteAsync(new DeleteDebitOriginCommand { Ids = [] }, default));
    }

    [Fact]
    public async Task Delete_EmptyGuidInIds_ThrowsValidationException()
    {
        var handler = new DeleteDebitOriginCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ExecuteAsync(new DeleteDebitOriginCommand { Ids = [Guid.Empty] }, default));
    }
}
