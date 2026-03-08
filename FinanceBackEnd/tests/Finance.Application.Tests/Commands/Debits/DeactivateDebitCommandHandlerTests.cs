using Finance.Application.Commands.Debits;
using Finance.Application.Services;
using Finance.Domain.Models.Debits;
using FluentValidation;

namespace Finance.Application.Tests.Commands.Debits;

public class DeactivateDebitCommandHandlerTests
{
    private readonly Mock<IEntityService<Debit, Guid>> _entityService;

    public DeactivateDebitCommandHandlerTests()
    {
        _entityService = new Mock<IEntityService<Debit, Guid>>();
    }

    [Fact]
    public async Task Deactivate_ValidIds_CallsSetDeactivatedWithTrueAndReturnsSuccess()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var command = new DeactivateDebitCommand { Ids = ids };

        var handler = new DeactivateDebitCommandHandler(_entityService.Object);

        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        _entityService.Verify(es => es.SetDeactivatedAsync(
            It.Is<ICollection<Guid>>(c => c.SequenceEqual(ids)),
            true,
            It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Deactivate_EmptyIds_ThrowsValidationException()
    {
        var command = new DeactivateDebitCommand { Ids = [] };

        var handler = new DeactivateDebitCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Deactivate_EmptyGuidInIds_ThrowsValidationException()
    {
        var command = new DeactivateDebitCommand { Ids = [Guid.Empty] };

        var handler = new DeactivateDebitCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }
}
