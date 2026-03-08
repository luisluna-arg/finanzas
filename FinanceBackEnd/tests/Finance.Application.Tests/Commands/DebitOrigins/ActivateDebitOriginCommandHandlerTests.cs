using Finance.Application.Commands.DebitOrigins;
using Finance.Application.Services;
using Finance.Domain.Models.Debits;
using FluentValidation;

namespace Finance.Application.Tests.Commands.DebitOrigins;

public class ActivateDebitOriginCommandHandlerTests
{
    private readonly Mock<IEntityService<DebitOrigin, Guid>> _entityService;

    public ActivateDebitOriginCommandHandlerTests()
    {
        _entityService = new Mock<IEntityService<DebitOrigin, Guid>>();
    }

    [Fact]
    public async Task Activate_ValidIds_CallsSetDeactivatedWithFalseAndReturnsSuccess()
    {
        var ids = new[] { Guid.NewGuid() };
        _entityService.Setup(s => s.SetDeactivatedAsync(It.IsAny<ICollection<Guid>>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DebitOrigin>());

        var handler = new ActivateDebitOriginCommandHandler(_entityService.Object);
        var result = await handler.ExecuteAsync(new ActivateDebitOriginCommand { Ids = ids }, default);

        Assert.True(result.IsSuccess);
        _entityService.Verify(s => s.SetDeactivatedAsync(ids, false, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Activate_EmptyIds_ThrowsValidationException()
    {
        var handler = new ActivateDebitOriginCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ExecuteAsync(new ActivateDebitOriginCommand { Ids = [] }, default));
    }

    [Fact]
    public async Task Activate_EmptyGuidInIds_ThrowsValidationException()
    {
        var handler = new ActivateDebitOriginCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ExecuteAsync(new ActivateDebitOriginCommand { Ids = [Guid.Empty] }, default));
    }
}
