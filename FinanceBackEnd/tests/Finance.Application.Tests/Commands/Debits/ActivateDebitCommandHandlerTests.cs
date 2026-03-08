using Finance.Application.Commands.Debits;
using Finance.Application.Services;
using Finance.Domain.Models.Debits;
using FluentValidation;

namespace Finance.Application.Tests.Commands.Debits;

public class ActivateDebitCommandHandlerTests
{
    private readonly Mock<IEntityService<Debit, Guid>> _entityService;

    public ActivateDebitCommandHandlerTests()
    {
        _entityService = new Mock<IEntityService<Debit, Guid>>();
    }

    [Fact]
    public async Task Activate_ValidIds_CallsSetDeactivatedWithFalseAndReturnsSuccess()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var command = new ActivateDebitCommand { Ids = ids };

        var handler = new ActivateDebitCommandHandler(_entityService.Object);

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
        var command = new ActivateDebitCommand { Ids = [] };

        var handler = new ActivateDebitCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Activate_EmptyGuidInIds_ThrowsValidationException()
    {
        var command = new ActivateDebitCommand { Ids = [Guid.Empty] };

        var handler = new ActivateDebitCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }
}
