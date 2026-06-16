using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Helpers.ExcelHelper;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Finance.Application.Commands.CreditCards;

public class ImportCreditCardStatementTransactionsCommandHandler : BaseResponselessHandler<ImportCreditCardStatementTransactionsCommand>
{
    public ImportCreditCardStatementTransactionsCommandHandler(FinanceDbContext db) : base(db) { }

    public override async Task<CommandResult> ExecuteAsync(
        ImportCreditCardStatementTransactionsCommand command, CancellationToken cancellationToken)
    {
        var template = await DbContext.CreditCardStatementImportTemplate
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == command.TemplateId, cancellationToken)
            ?? throw new Exception($"Import template not found: {command.TemplateId}");

        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var config = JsonSerializer.Deserialize<StatementImportConfig>(template.ConfigJson, jsonOptions)
            ?? throw new Exception("Invalid template configuration");

        if (config.DefaultCurrencyId == Guid.Empty)
        {
            using var doc = JsonDocument.Parse(template.ConfigJson);
            if (doc.RootElement.TryGetProperty("config", out var configEl))
                config = JsonSerializer.Deserialize<StatementImportConfig>(configEl.GetRawText(), jsonOptions)
                    ?? config;
        }

        var statement = await DbContext.CreditCardStatement
            .Include(s => s.CreditCard)
            .FirstOrDefaultAsync(s => s.Id == command.StatementId, cancellationToken)
            ?? throw new Exception($"Statement not found: {command.StatementId}");

        var helper = new StatementImportExcelHelper();
        var rows = helper.Read(command.File, config).ToArray();
        if (rows.Length == 0) return CommandResult.Failure("No rows found in the uploaded file.");

        var existingTxs = await DbContext.CreditCardTransaction
            .Where(t => t.CreditCardId == statement.CreditCardId)
            .Select(t => new { t.Timestamp, t.Concept, Amount = (decimal)t.Amount })
            .ToArrayAsync(cancellationToken);

        var existingSet = existingTxs
            .Select(t => (t.Timestamp.Date, t.Concept, t.Amount))
            .ToHashSet();

        foreach (var row in rows)
        {
            if (existingSet.Contains((row.Date.Date, row.Concept, row.Amount)))
                continue;

            var tx = new CreditCardTransaction
            {
                CreditCardId = statement.CreditCardId,
                Timestamp = row.Date,
                TransactionType = CreditCardTransactionType.Purchase,
                Concept = row.Concept,
                Amount = row.Amount,
                CurrencyId = config.DefaultCurrencyId,
            };
            DbContext.CreditCardTransaction.Add(tx);

            DbContext.CreditCardStatementTransaction.Add(new CreditCardStatementTransaction
            {
                StatementId = statement.Id,
                CreditCardTransactionId = tx.Id,
                PostedDate = row.Date,
                Amount = row.Amount,
                Description = row.Concept,
                CurrencyId = config.DefaultCurrencyId,
            });
        }

        await DbContext.SaveChangesAsync(cancellationToken);

        var templateForLink = await DbContext.CreditCardStatementImportTemplate
            .IgnoreQueryFilters()
            .Include(t => t.CreditCards)
            .FirstAsync(t => t.Id == command.TemplateId, cancellationToken);

        if (!templateForLink.CreditCards.Any(c => c.Id == statement.CreditCardId))
        {
            templateForLink.CreditCards.Add(statement.CreditCard!);
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        return CommandResult.Success();
    }
}

public class ImportCreditCardStatementTransactionsCommand : ICommand
{
    public IFormFile File { get; set; } = default!;
    public Guid TemplateId { get; set; }
    public Guid StatementId { get; set; }
}

