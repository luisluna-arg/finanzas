using Finance.Application.Commands.Debits;
using Finance.Application.Services;
using Finance.Domain.Models.Debits;
using FluentValidation;

namespace Finance.Application.Tests.Commands.Debits;

public class DeleteDebitCommandHandlerTests
{
    private readonly Mock<IEntityService<Debit, Guid>> _entityService;

    public DeleteDebitCommandHandlerTests()
    {
        _entityService = new Mock<IEntityService<Debit, Guid>>();
    }

    [Fact]
    public async Task Delete_ValidIds_CallsDeleteServiceAndReturnsSuccess()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var command = new DeleteDebitCommand { Ids = ids };

        var handler = new DeleteDebitCommandHandler(_entityService.Object);

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
        var command = new DeleteDebitCommand { Ids = [] };

        var handler = new DeleteDebitCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Delete_EmptyGuidInIds_ThrowsValidationException()
    {
        var command = new DeleteDebitCommand { Ids = [Guid.Empty] };

        var handler = new DeleteDebitCommandHandler(_entityService.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }
}
