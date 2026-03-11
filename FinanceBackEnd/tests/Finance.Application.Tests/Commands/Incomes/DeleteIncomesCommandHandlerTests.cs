using Finance.Application.Commands.Incomes;
using Finance.Application.Repositories;
using Finance.Domain.Models.Incomes;

namespace Finance.Application.Tests.Commands.Incomes;

public sealed class DeleteIncomesCommandHandlerTests
{
    private readonly Mock<IRepository<Income, Guid>> _service = new();

    [Fact]
    public async Task Delete_HappyPath_CallsServiceDeleteAndReturnsSuccess()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var command = new DeleteIncomesCommand() { Ids = ids };

        _service.Setup(s => s.DeleteAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>(),
            false))
            .Returns(Task.CompletedTask);

        var handler = new DeleteIncomesCommandHandler(_service.Object);
        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        _service.Verify(s => s.DeleteAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>(),
            false),
            Times.Exactly(ids.Length));
        _service.Verify(s => s.PersistAsync(
            It.IsAny<CancellationToken>()),
            Times.Once());
    }
}
