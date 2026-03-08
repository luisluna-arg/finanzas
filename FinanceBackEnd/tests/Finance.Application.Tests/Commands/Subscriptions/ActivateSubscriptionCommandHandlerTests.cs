using Finance.Application.Commands.Subscriptions;
using Finance.Application.Services;
using Finance.Domain.Models.Subscriptions;
using FluentValidation;

namespace Finance.Application.Tests.Commands.Subscriptions;

public class ActivateSubscriptionCommandHandlerTests
{
    private readonly Mock<IEntityService<Subscription, Guid>> _entityService;

    public ActivateSubscriptionCommandHandlerTests()
    {
        _entityService = new Mock<IEntityService<Subscription, Guid>>();
    }

    [Fact]
    public async Task Activate_ValidIds_CallsSetDeactivatedWithFalseAndReturnsSuccess()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var command = new ActivateSubscriptionCommand { Ids = ids };

        var handler = new ActivateSubscriptionCommandHandler(_entityService.Object);

        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        _entityService.Verify(es => es.SetDeactivatedAsync(
            It.Is<ICollection<Guid>>(c => c.SequenceEqual(ids)),
            false,
            It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Activate_EmptyIds_ThrowsValidationException()
    {
        var command = new ActivateSubscriptionCommand { Ids = [] };

        var handler = new ActivateSubscriptionCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Activate_EmptyGuidInIds_ThrowsValidationException()
    {
        var command = new ActivateSubscriptionCommand { Ids = [Guid.Empty] };

        var handler = new ActivateSubscriptionCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }
}
