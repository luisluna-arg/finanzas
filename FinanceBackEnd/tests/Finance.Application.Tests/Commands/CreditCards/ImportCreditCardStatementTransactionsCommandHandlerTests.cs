using Finance.Application.Commands.CreditCards;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.Models.Identities;
using Finance.Domain.SpecialTypes;
using Finance.Persistence.Constants;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace Finance.Application.Tests.Commands.CreditCards;

public class ImportCreditCardStatementTransactionsCommandHandlerTests : QueryHandlerBaseTests
{
    static ImportCreditCardStatementTransactionsCommandHandlerTests()
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }

    private ImportCreditCardStatementTransactionsCommandHandler CreateHandler() =>
        new(_dbContext);

    private static IFormFile BuildCsvFile(string content, string fileName = "test.csv")
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        var file = new Mock<IFormFile>();
        file.Setup(f => f.FileName).Returns(fileName);
        file.Setup(f => f.Length).Returns(stream.Length);
        file.Setup(f => f.OpenReadStream()).Returns(stream);
        return file.Object;
    }

    private async Task<(CreditCard creditCard, CreditCardStatement statement, CreditCardStatementImportTemplate template)> SeedRequiredData()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "u",
            FirstName = "F",
            LastName = "L",
            Identities = [new Identity { SourceId = "IdentityNotFound" }],
        };
        await _dbContext.User.AddAsync(user);

        var creditCard = new CreditCard { Id = Guid.NewGuid(), Name = "Visa" };
        await _dbContext.CreditCard.AddAsync(creditCard);
        await _dbContext.SaveChangesAsync();

        _dbContext.CreditCardPermissions.Add(new CreditCardPermissions
        {
            ResourceId = creditCard.Id,
            Resource = creditCard,
            UserId = user.Id,
            User = user,
            PermissionLevels = [PermissionLevelEnum.Owner],
        });

        var statement = new CreditCardStatement
        {
            Id = Guid.NewGuid(),
            CreditCardId = creditCard.Id,
            ClosureDate = new DateTime(2025, 6, 30, 0, 0, 0, DateTimeKind.Utc),
            ExpiringDate = new DateTime(2025, 7, 10, 0, 0, 0, DateTimeKind.Utc),
        };
        await _dbContext.CreditCardStatement.AddAsync(statement);

        var config = new StatementImportConfig
        {
            SkipRows = 1,
            DateColumn = 0,
            DateFormat = "d/M/yyyy",
            ConceptColumn = 1,
            AmountColumn = 2,
            DefaultCurrencyId = Guid.Parse(CurrencyConstants.PesoId),
            DecimalSeparator = ",",
            ThousandsSeparator = ".",
        };
        var template = new CreditCardStatementImportTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Test Template",
            IsSystem = true,
            ConfigJson = JsonSerializer.Serialize(config),
        };
        await _dbContext.CreditCardStatementImportTemplate.AddAsync(template);
        await _dbContext.SaveChangesAsync();

        return (creditCard, statement, template);
    }

    [Fact]
    public async Task Import_WithValidCsvFile_CreatesTransactionsAndStatementTransactions()
    {
        var (_, statement, template) = await SeedRequiredData();
        var csv = "Date,Concept,Amount\r\n1/6/2025,Netflix,1500\r\n5/6/2025,Uber,300";
        var command = new ImportCreditCardStatementTransactionsCommand
        {
            File = BuildCsvFile(csv),
            TemplateId = template.Id,
            StatementId = statement.Id,
        };

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        var txs = _dbContext.CreditCardTransaction.IgnoreQueryFilters()
            .Where(t => t.CreditCardId == statement.CreditCardId).ToList();
        Assert.Equal(2, txs.Count);
        var stmtTxs = _dbContext.CreditCardStatementTransaction.IgnoreQueryFilters()
            .Where(st => st.StatementId == statement.Id).ToList();
        Assert.Equal(2, stmtTxs.Count);
    }

    [Fact]
    public async Task Import_WhenTemplateNotFound_ThrowsException()
    {
        var (_, statement, _) = await SeedRequiredData();
        var command = new ImportCreditCardStatementTransactionsCommand
        {
            File = BuildCsvFile("Date,Concept,Amount\r\n1/6/2025,Netflix,1500"),
            TemplateId = Guid.NewGuid(),
            StatementId = statement.Id,
        };

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Import_WhenStatementNotFound_ThrowsException()
    {
        var (_, _, template) = await SeedRequiredData();
        var command = new ImportCreditCardStatementTransactionsCommand
        {
            File = BuildCsvFile("Date,Concept,Amount\r\n1/6/2025,Netflix,1500"),
            TemplateId = template.Id,
            StatementId = Guid.NewGuid(),
        };

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().ExecuteAsync(command, default));
    }

    [Fact]
    public async Task Import_WhenFileHasNoDataRows_ReturnsFailure()
    {
        var (_, statement, template) = await SeedRequiredData();
        var command = new ImportCreditCardStatementTransactionsCommand
        {
            File = BuildCsvFile("Date,Concept,Amount"),
            TemplateId = template.Id,
            StatementId = statement.Id,
        };

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Import_WithDuplicateExistingTransaction_SkipsDuplicate()
    {
        var (creditCard, statement, template) = await SeedRequiredData();

        var existingTx = new CreditCardTransaction
        {
            Id = Guid.NewGuid(),
            CreditCardId = creditCard.Id,
            Timestamp = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),
            Concept = "Netflix",
            Amount = 1500m,
            TransactionType = CreditCardTransactionType.Purchase,
            CurrencyId = Guid.Parse(CurrencyConstants.PesoId),
        };
        _dbContext.CreditCardTransaction.Add(existingTx);
        await _dbContext.SaveChangesAsync();

        var csv = "Date,Concept,Amount\r\n1/6/2025,Netflix,1500\r\n5/6/2025,Uber,300";
        var command = new ImportCreditCardStatementTransactionsCommand
        {
            File = BuildCsvFile(csv),
            TemplateId = template.Id,
            StatementId = statement.Id,
        };

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        var importedTxs = _dbContext.CreditCardTransaction.IgnoreQueryFilters()
            .Where(t => t.CreditCardId == creditCard.Id && t.Id != existingTx.Id).ToList();
        Assert.Single(importedTxs);
        Assert.Equal("Uber", importedTxs[0].Concept);
    }

    [Fact]
    public async Task Import_SetsCorrectTransactionFields()
    {
        var (_, statement, template) = await SeedRequiredData();
        var csv = "Date,Concept,Amount\r\n15/6/2025,Supermercado,4500";
        var command = new ImportCreditCardStatementTransactionsCommand
        {
            File = BuildCsvFile(csv),
            TemplateId = template.Id,
            StatementId = statement.Id,
        };

        await CreateHandler().ExecuteAsync(command, default);

        var tx = _dbContext.CreditCardTransaction.IgnoreQueryFilters()
            .Single(t => t.CreditCardId == statement.CreditCardId);
        Assert.Equal(new DateTime(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc), tx.Timestamp);
        Assert.Equal("Supermercado", tx.Concept);
        Assert.Equal(new Money(4500m), tx.Amount);
        Assert.Equal(CreditCardTransactionType.Purchase, tx.TransactionType);
        Assert.Equal(Guid.Parse(CurrencyConstants.PesoId), tx.CurrencyId);
    }
}
