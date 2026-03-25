using Finance.Application.Queries.CreditCards;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Banks;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.Models.Identities;
using Finance.Domain.SpecialTypes;
using FinanceBackEnd.Finance.Domain.Enums;

namespace Finance.Application.Tests.Queries.CreditCards;

public class GetCreditCardTransactionsQueryHandlerTests : QueryHandlerBaseTests
{
    private GetCreditCardTransactionsQueryHandler CreateHandler() => new(_dbContext);

    private async Task<(CreditCard card, CreditCardTransaction tx)> SeedTransactionAsync(
        string concept = "Test",
        decimal amount = 100m,
        bool deactivated = false,
        DateTime? timestamp = null)
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
        var tx = new CreditCardTransaction
        {
            Id = Guid.NewGuid(),
            CreditCardId = card.Id,
            Timestamp = timestamp ?? new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc),
            TransactionType = CreditCardTransactionType.Purchase,
            Concept = concept,
            Amount = new Money(amount),
            Deactivated = deactivated,
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
        await _dbContext.CreditCardTransaction.AddAsync(tx);
        await _dbContext.SaveChangesAsync();
        return (card, tx);
    }

    [Fact]
    public async Task Execute_WithNoFilters_ReturnsAllActiveTransactions()
    {
        var (_, tx1) = await SeedTransactionAsync("Tx1", 100m);
        var (_, tx2) = await SeedTransactionAsync("Tx2", 200m);

        var result = await CreateHandler().ExecuteAsync(new GetCreditCardTransactionsQuery(), default);

        Assert.True(result.IsSuccess);
        var myIds = new[] { tx1.Id, tx2.Id };
        var mine = result.Data.Where(t => myIds.Contains(t.Id)).ToList();
        Assert.Equal(2, mine.Count);
    }

    [Fact]
    public async Task Execute_WithStatementIdFilter_ReturnsOnlyStatementTransactions()
    {
        var (card1, txLinked) = await SeedTransactionAsync("Linked", 100m);
        var (_, txOther) = await SeedTransactionAsync("Other", 200m);

        var statement = new CreditCardStatement
        {
            Id = Guid.NewGuid(),
            CreditCardId = card1.Id,
            ClosureDate = new DateTime(2025, 3, 31, 0, 0, 0, DateTimeKind.Utc),
            ExpiringDate = new DateTime(2025, 4, 15, 0, 0, 0, DateTimeKind.Utc),
        };
        _dbContext.CreditCardStatement.Add(statement);
        _dbContext.CreditCardStatementTransaction.Add(new CreditCardStatementTransaction
        {
            Id = Guid.NewGuid(),
            StatementId = statement.Id,
            CreditCardTransactionId = txLinked.Id,
            PostedDate = txLinked.Timestamp,
            Amount = txLinked.Amount,
            Description = txLinked.Concept,
        });
        await _dbContext.SaveChangesAsync();

        var result = await CreateHandler().ExecuteAsync(
            new GetCreditCardTransactionsQuery { StatementId = statement.Id }, default);

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Data, t => t.Id == txLinked.Id);
        Assert.DoesNotContain(result.Data, t => t.Id == txOther.Id);
    }

    [Fact]
    public async Task Execute_WithCreditCardIdFilter_ReturnsOnlyMatchingCard()
    {
        var (card1, tx1) = await SeedTransactionAsync("Tx1", 100m);
        var (_, tx2) = await SeedTransactionAsync("Tx2", 200m);

        var result = await CreateHandler().ExecuteAsync(
            new GetCreditCardTransactionsQuery { CreditCardId = card1.Id }, default);

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Data, t => t.Id == tx1.Id);
        Assert.DoesNotContain(result.Data, t => t.Id == tx2.Id);
    }

    [Fact]
    public async Task Execute_WhenIncludeDeactivatedIsFalse_ExcludesDeactivatedTransactions()
    {
        var (_, activeTx) = await SeedTransactionAsync("Active", 100m, deactivated: false);
        var (_, deactivatedTx) = await SeedTransactionAsync("Deactivated", 200m, deactivated: true);

        var result = await CreateHandler().ExecuteAsync(new GetCreditCardTransactionsQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Data, t => t.Id == activeTx.Id);
        Assert.DoesNotContain(result.Data, t => t.Id == deactivatedTx.Id);
    }
}
