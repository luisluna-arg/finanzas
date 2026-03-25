using Finance.Application.Queries.CreditCards;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Banks;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.Models.Identities;
using Finance.Domain.SpecialTypes;
using FinanceBackEnd.Finance.Domain.Enums;

namespace Finance.Application.Tests.Queries.CreditCards;

public class GetLatestCreditCardTransactionsFromStatementsQueryHandlerTests : QueryHandlerBaseTests
{
    private GetLatestCreditCardTransactionsFromStatementsQueryHandler CreateHandler() => new(_dbContext);

    private async Task<(CreditCard card, CreditCardStatement statement, CreditCardTransaction tx)> SeedAsync(
        DateTime closureDate,
        bool statementDeactivated = false,
        bool txDeactivated = false)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "u",
            FirstName = "F",
            LastName = "L",
            Identities = [new Identity { SourceId = "IdentityNotFound" }],
        };
        var bank = new Bank { Id = Guid.NewGuid(), Name = "Bank" };
        var card = new CreditCard { Id = Guid.NewGuid(), Name = "Card", BankId = bank.Id };
        var statement = new CreditCardStatement
        {
            Id = Guid.NewGuid(),
            CreditCardId = card.Id,
            ClosureDate = closureDate,
            ExpiringDate = closureDate.AddDays(15),
            Deactivated = statementDeactivated,
        };
        var tx = new CreditCardTransaction
        {
            Id = Guid.NewGuid(),
            CreditCardId = card.Id,
            Timestamp = closureDate.AddDays(-5),
            TransactionType = CreditCardTransactionType.Purchase,
            Concept = "Purchase",
            Amount = new Money(100m),
            Deactivated = txDeactivated,
        };
        var statementTx = new CreditCardStatementTransaction
        {
            Id = Guid.NewGuid(),
            StatementId = statement.Id,
            CreditCardTransactionId = tx.Id,
            PostedDate = tx.Timestamp,
            Amount = tx.Amount,
            Description = tx.Concept,
        };
        await _dbContext.User.AddAsync(user);
        await _dbContext.Bank.AddAsync(bank);
        await _dbContext.CreditCard.AddAsync(card);
        _dbContext.CreditCardPermissions.Add(new CreditCardPermissions
        {
            ResourceId = card.Id,
            Resource = card,
            UserId = user.Id,
            User = user,
            PermissionLevels = [PermissionLevelEnum.Owner],
        });
        await _dbContext.CreditCardStatement.AddAsync(statement);
        await _dbContext.CreditCardTransaction.AddAsync(tx);
        _dbContext.CreditCardStatementTransaction.Add(statementTx);
        await _dbContext.SaveChangesAsync();
        return (card, statement, tx);
    }

    [Fact]
    public async Task Execute_WithDeactivatedStatement_DoesNotReturnItsTransactions()
    {
        var closureDate = new DateTime(2025, 1, 31, 0, 0, 0, DateTimeKind.Utc);
        var (card, _, tx) = await SeedAsync(closureDate, statementDeactivated: true);

        var result = await CreateHandler().ExecuteAsync(
            new GetLatestCreditCardTransactionsFromStatementsQuery { CreditCardId = card.Id }, default);

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(result.Data, t => t.Id == tx.Id);
    }

    [Fact]
    public async Task Execute_WithMultipleStatementsForSameCard_ReturnsLatestStatementTransactions()
    {
        var olderClosureDate = new DateTime(2025, 1, 31, 0, 0, 0, DateTimeKind.Utc);
        var newerClosureDate = new DateTime(2025, 2, 28, 0, 0, 0, DateTimeKind.Utc);

        var (card, _, olderTx) = await SeedAsync(olderClosureDate);

        var newerStatement = new CreditCardStatement
        {
            Id = Guid.NewGuid(),
            CreditCardId = card.Id,
            ClosureDate = newerClosureDate,
            ExpiringDate = newerClosureDate.AddDays(15),
        };
        var newerTx = new CreditCardTransaction
        {
            Id = Guid.NewGuid(),
            CreditCardId = card.Id,
            Timestamp = new DateTime(2025, 2, 20, 0, 0, 0, DateTimeKind.Utc),
            TransactionType = CreditCardTransactionType.Purchase,
            Concept = "Newer purchase",
            Amount = new Money(200m),
        };
        _dbContext.CreditCardStatement.Add(newerStatement);
        _dbContext.CreditCardTransaction.Add(newerTx);
        _dbContext.CreditCardStatementTransaction.Add(new CreditCardStatementTransaction
        {
            Id = Guid.NewGuid(),
            StatementId = newerStatement.Id,
            CreditCardTransactionId = newerTx.Id,
            PostedDate = newerTx.Timestamp,
            Amount = newerTx.Amount,
            Description = newerTx.Concept,
        });
        await _dbContext.SaveChangesAsync();

        var result = await CreateHandler().ExecuteAsync(
            new GetLatestCreditCardTransactionsFromStatementsQuery { CreditCardId = card.Id }, default);

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Data, t => t.Id == newerTx.Id);
        Assert.DoesNotContain(result.Data, t => t.Id == olderTx.Id);
    }

    [Fact]
    public async Task Execute_WithCreditCardIdFilter_ReturnsOnlyMatchingCard()
    {
        var closureDate = new DateTime(2025, 2, 28, 0, 0, 0, DateTimeKind.Utc);
        var (card1, _, tx1) = await SeedAsync(closureDate);
        var (_, _, tx2) = await SeedAsync(closureDate);

        var result = await CreateHandler().ExecuteAsync(
            new GetLatestCreditCardTransactionsFromStatementsQuery { CreditCardId = card1.Id }, default);

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Data, t => t.Id == tx1.Id);
        Assert.DoesNotContain(result.Data, t => t.Id == tx2.Id);
    }

    [Fact]
    public async Task Execute_WhenTransactionIsDeactivated_ExcludesItFromDefaultResult()
    {
        var closureDate = new DateTime(2025, 2, 28, 0, 0, 0, DateTimeKind.Utc);
        var (_, _, activeTx) = await SeedAsync(closureDate, txDeactivated: false);
        var (_, _, deactivatedTx) = await SeedAsync(closureDate, txDeactivated: true);

        var result = await CreateHandler().ExecuteAsync(
            new GetLatestCreditCardTransactionsFromStatementsQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Data, t => t.Id == activeTx.Id);
        Assert.DoesNotContain(result.Data, t => t.Id == deactivatedTx.Id);
    }
}
