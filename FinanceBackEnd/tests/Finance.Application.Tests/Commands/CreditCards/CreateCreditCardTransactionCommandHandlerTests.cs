using Finance.Application.Commands.CreditCards;
using Finance.Application.Repositories;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.SpecialTypes;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Tests.Commands.CreditCards;

public class CreateCreditCardTransactionCommandHandlerTests : QueryHandlerBaseTests
{
    private readonly Mock<IRepository<CreditCard, Guid>> _creditCardRepo;
    private readonly Mock<IRepository<CreditCardTransaction, Guid>> _transactionRepo;

    public CreateCreditCardTransactionCommandHandlerTests()
    {
        _creditCardRepo = new Mock<IRepository<CreditCard, Guid>>();
        _transactionRepo = new Mock<IRepository<CreditCardTransaction, Guid>>();
    }

    private CreateCreditCardTransactionCommandHandler CreateHandler() =>
        new(_dbContext, _creditCardRepo.Object, _transactionRepo.Object);

    [Fact]
    public async Task Create_WithoutStatementId_AddsTransactionAndReturnsSuccess()
    {
        var creditCard = new CreditCard { Id = Guid.NewGuid(), Name = "Visa" };
        var command = new CreateCreditCardTransactionCommand
        {
            CreditCardId = creditCard.Id,
            Timestamp = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc),
            TransactionType = CreditCardTransactionType.Purchase,
            Concept = "Supermercado",
            Amount = new Money(1500m),
        };

        _creditCardRepo
            .Setup(r => r.GetByIdAsync(creditCard.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(creditCard);
        _transactionRepo
            .Setup(r => r.AddAsync(It.IsAny<CreditCardTransaction>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(creditCard.Id, result.Data.CreditCardId);
        Assert.Equal("Supermercado", result.Data.Concept);
        _transactionRepo.Verify(
            r => r.AddAsync(It.IsAny<CreditCardTransaction>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WithStatementId_CreatesStatementTransactionInDb()
    {
        var creditCard = new CreditCard { Id = Guid.NewGuid(), Name = "Mastercard" };
        var statementId = Guid.NewGuid();
        var command = new CreateCreditCardTransactionCommand
        {
            CreditCardId = creditCard.Id,
            CreditCardStatementId = statementId,
            Timestamp = new DateTime(2025, 3, 5, 0, 0, 0, DateTimeKind.Utc),
            TransactionType = CreditCardTransactionType.Purchase,
            Concept = "Farmacia",
            Amount = new Money(250m),
        };

        _creditCardRepo
            .Setup(r => r.GetByIdAsync(creditCard.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(creditCard);
        _transactionRepo
            .Setup(r => r.AddAsync(It.IsAny<CreditCardTransaction>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
            .Callback<CreditCardTransaction, CancellationToken, bool>((tx, _, _) => tx.Id = Guid.NewGuid())
            .Returns(Task.CompletedTask);

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        var statementTx = _dbContext.CreditCardStatementTransaction
            .IgnoreQueryFilters()
            .Where(st => st.StatementId == statementId)
            .ToList();
        Assert.Single(statementTx);
        Assert.Equal("Farmacia", statementTx[0].Description);
        _transactionRepo.Verify(
            r => r.AddAsync(It.IsAny<CreditCardTransaction>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenCreditCardNotFound_ThrowsException()
    {
        var command = new CreateCreditCardTransactionCommand
        {
            CreditCardId = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            TransactionType = CreditCardTransactionType.Purchase,
            Concept = "Test",
            Amount = new Money(100m),
        };

        _creditCardRepo
            .Setup(r => r.GetByIdAsync(command.CreditCardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreditCard?)null);

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().ExecuteAsync(command, default));
    }
}
