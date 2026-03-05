using Finance.Application.Legacy.Commands.Subscriptions;
using Finance.Application.Legacy.Services;
using Finance.Domain.Models.Subscriptions;
using FluentValidation;

namespace Finance.Application.Tests.Commands.Subscriptions;

public class DeactivateSubscriptionCommandHandlerTests
{
    private readonly Mock<IEntityService<Subscription, Guid>> _entityService;

    public DeactivateSubscriptionCommandHandlerTests()
    {
        _entityService = new Mock<IEntityService<Subscription, Guid>>();
    }

    [Fact]
    public async Task Deactivate_ValidIds_CallsSetDeactivatedWithTrueAndReturnsSuccess()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var command = new DeactivateSubscriptionCommand { Ids = ids };

        var handler = new DeactivateSubscriptionCommandHandler(_entityService.Object);

        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        _entityService.Verify(es => es.SetDeactivatedAsync(
            It.Is<ICollection<Guid>>(c => c.SequenceEqual(ids)),
            true,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Deactivate_EmptyIds_ThrowsValidationException()
    {
        var command = new DeactivateSubscriptionCommand { Ids = [] };

        var handler = new DeactivateSubscriptionCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Deactivate_EmptyGuidInIds_ThrowsValidationException()
    {
        var command = new DeactivateSubscriptionCommand { Ids = [Guid.Empty] };

        var handler = new DeactivateSubscriptionCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }
}
