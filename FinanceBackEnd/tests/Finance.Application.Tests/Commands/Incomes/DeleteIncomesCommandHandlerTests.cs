using Finance.Application.Commands.Incomes;
using Finance.Application.Services;
using Finance.Domain.Models.Incomes;

namespace Finance.Application.Tests.Commands.Incomes;

public class DeleteIncomesCommandHandlerTests
{
    private readonly Mock<IEntityService<Income, Guid>> _service;

    public DeleteIncomesCommandHandlerTests()
    {
        _service = new Mock<IEntityService<Income, Guid>>();
    }

    [Fact]
    public async Task Delete_HappyPath_CallsServiceDeleteAndReturnsSuccess()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var command = new DeleteIncomesCommand(ids);

        _service.Setup(s => s.DeleteAsync(It.IsAny<ICollection<Guid>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new DeleteIncomesCommandHandler(_service.Object);
        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        _service.Verify(s => s.DeleteAsync(
            It.Is<ICollection<Guid>>(c => c.SequenceEqual(ids)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
