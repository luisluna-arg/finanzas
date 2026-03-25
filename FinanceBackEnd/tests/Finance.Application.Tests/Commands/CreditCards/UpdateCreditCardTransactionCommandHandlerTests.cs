using Finance.Application.Commands.CreditCards;
using Finance.Application.Repositories;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Tests.Commands.CreditCards;

public class UpdateCreditCardTransactionCommandHandlerTests : QueryHandlerBaseTests
{
    private readonly Mock<IRepository<CreditCardTransaction, Guid>> _transactionRepo;

    public UpdateCreditCardTransactionCommandHandlerTests()
    {
        _transactionRepo = new Mock<IRepository<CreditCardTransaction, Guid>>();
    }

    private UpdateCreditCardTransactionCommandHandler CreateHandler() =>
        new(_transactionRepo.Object, _dbContext);

    [Fact]
    public async Task Update_HappyPath_UpdatesFieldsAndReturnsSuccess()
    {
        var transaction = new CreditCardTransaction
        {
            Id = Guid.NewGuid(),
            Timestamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Concept = "Original concept",
            Amount = new Money(100m),
        };
        var command = new UpdateCreditCardTransactionCommand
        {
            Id = transaction.Id,
            Timestamp = new DateTime(2025, 3, 10, 0, 0, 0, DateTimeKind.Utc),
            Concept = "Updated concept",
            Amount = new Money(350m),
        };

        _transactionRepo
            .Setup(r => r.GetByIdAsync(transaction.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(command.Timestamp, result.Data.Timestamp);
        Assert.Equal("Updated concept", result.Data.Concept);
        Assert.Equal(350m, (decimal)result.Data.Amount);
        _transactionRepo.Verify(
            r => r.UpdateAsync(transaction, It.IsAny<CancellationToken>(), It.IsAny<bool>()),
            Times.Once);
        _transactionRepo.Verify(r => r.PersistAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_WhenTransactionNotFound_ThrowsException()
    {
        var command = new UpdateCreditCardTransactionCommand
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            Concept = "Test",
            Amount = new Money(100m),
        };

        _transactionRepo
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreditCardTransaction?)null);

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().ExecuteAsync(command, default));
    }
}
