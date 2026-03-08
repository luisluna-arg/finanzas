using Finance.Application.Commands.DebitOrigins;
using Finance.Application.Services;
using Finance.Domain.Models.Debits;
using FluentValidation;

namespace Finance.Application.Tests.Commands.DebitOrigins;

public class DeactivateDebitOriginCommandHandlerTests
{
    private readonly Mock<IEntityService<DebitOrigin, Guid>> _entityService;

    public DeactivateDebitOriginCommandHandlerTests()
    {
        _entityService = new Mock<IEntityService<DebitOrigin, Guid>>();
    }

    [Fact]
    public async Task Deactivate_ValidIds_CallsSetDeactivatedWithTrueAndReturnsSuccess()
    {
        var ids = new[] { Guid.NewGuid() };
        _entityService.Setup(s => s.SetDeactivatedAsync(It.IsAny<ICollection<Guid>>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DebitOrigin>());

        var handler = new DeactivateDebitOriginCommandHandler(_entityService.Object);
        var result = await handler.ExecuteAsync(new DeactivateDebitOriginCommand { Ids = ids }, default);

        Assert.True(result.IsSuccess);
        _entityService.Verify(s => s.SetDeactivatedAsync(ids, true, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Deactivate_EmptyIds_ThrowsValidationException()
    {
        var handler = new DeactivateDebitOriginCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ExecuteAsync(new DeactivateDebitOriginCommand { Ids = [] }, default));
    }

    [Fact]
    public async Task Deactivate_EmptyGuidInIds_ThrowsValidationException()
    {
        var handler = new DeactivateDebitOriginCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ExecuteAsync(new DeactivateDebitOriginCommand { Ids = [Guid.Empty] }, default));
    }
}
