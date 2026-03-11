using Finance.Application.Commands.Debits;
using Finance.Application.Repositories;
using Finance.Domain.Models.Debits;
using FluentValidation;

namespace Finance.Application.Tests.Commands.Debits;

public sealed class DeleteDebitCommandHandlerTests
{
    private readonly Mock<IRepository<Debit, Guid>> _repository = new();

    [Fact]
    public async Task Delete_ValidIds_CallsDeleteServiceAndReturnsSuccess()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var command = new DeleteDebitCommand { Ids = ids };

        var handler = new DeleteDebitCommandHandler(_repository.Object);

        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        _repository.Verify(es => es.DeleteAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>(),
            false),
            Times.Exactly(ids.Length));
        _repository.Verify(es => es.PersistAsync(
            It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task Delete_EmptyIds_ThrowsValidationException()
    {
        var command = new DeleteDebitCommand { Ids = [] };

        var handler = new DeleteDebitCommandHandler(_repository.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Delete_EmptyGuidInIds_ThrowsValidationException()
    {
        var command = new DeleteDebitCommand { Ids = [Guid.Empty] };

        var handler = new DeleteDebitCommandHandler(_repository.Object);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ExecuteAsync(command, default));
    }
}
