using Finance.Application.Legacy.Commands.Subscriptions;
using Finance.Application.Legacy.Services;
using Finance.Domain.Models.Subscriptions;
using FluentValidation;

namespace Finance.Application.Tests.Commands.Subscriptions;

public class DeleteSubscriptionCommandHandlerTests
{
    private readonly Mock<IEntityService<Subscription, Guid>> _entityService;

    public DeleteSubscriptionCommandHandlerTests()
    {
        _entityService = new Mock<IEntityService<Subscription, Guid>>();
    }

    [Fact]
    public async Task Delete_ValidIds_CallsDeleteServiceAndReturnsSuccess()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var command = new DeleteSubscriptionCommand { Ids = ids };

        var handler = new DeleteSubscriptionCommandHandler(_entityService.Object);

        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        _entityService.Verify(es => es.DeleteAsync(
            It.Is<ICollection<Guid>>(c => c.SequenceEqual(ids)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Delete_EmptyIds_ThrowsValidationException()
    {
        var command = new DeleteSubscriptionCommand { Ids = [] };

        var handler = new DeleteSubscriptionCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Delete_EmptyGuidInIds_ThrowsValidationException()
    {
        var command = new DeleteSubscriptionCommand { Ids = [Guid.Empty] };

        var handler = new DeleteSubscriptionCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }
}
