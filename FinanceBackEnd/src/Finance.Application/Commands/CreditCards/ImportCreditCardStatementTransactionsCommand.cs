using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Helpers.ExcelHelper;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        };
        var config = JsonSerializer.Deserialize<StatementImportConfig>(template.ConfigJson, jsonOptions)
            ?? throw new Exception("Invalid template configuration");

        var statement = await DbContext.CreditCardStatement
            .Include(s => s.CreditCard)
            .FirstOrDefaultAsync(s => s.Id == command.StatementId, cancellationToken)
            ?? throw new Exception($"Statement not found: {command.StatementId}");

        var extension = Path.GetExtension(command.File.FileName).ToLowerInvariant();
        IEnumerable<StatementImportRow> rawRows = extension == ".pdf" || config.SourceType == SourceType.Pdf
            ? new StatementImportPdfHelper().Read(command.File, config)
            : new StatementImportExcelHelper().Read(command.File, config);

        var rows = rawRows.ToArray();
        if (rows.Length == 0)
        {
            if (extension == ".pdf" || config.SourceType == SourceType.Pdf)
            {
                var diag = new StatementImportPdfHelper().Diagnose(command.File);
                var sample = diag.Pages
                    .Skip(config.SkipPages)
                    .Take(1)
                    .SelectMany(p => p.Lines.Take(15))
                    .Select(l => $"[{string.Join(", ", l.Words.Select(w => $"{w.Text}@x{w.XCenter:F2}"))}]");
                return CommandResult.Failure(
                    $"No rows found. Pages={diag.PageCount}, skipPages={config.SkipPages}. " +
                    $"First lines of page {config.SkipPages + 1}: {string.Join(" | ", sample)}");
            }
            return CommandResult.Failure("No rows found in the uploaded file.");
        }

        var existingTxs = await DbContext.CreditCardTransaction
            .Where(t => t.CreditCardId == statement.CreditCardId && !t.Deactivated)
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
                CurrencyId = row.CurrencyId,
            };
            DbContext.CreditCardTransaction.Add(tx);

            DbContext.CreditCardStatementTransaction.Add(new CreditCardStatementTransaction
            {
                StatementId = statement.Id,
                CreditCardTransactionId = tx.Id,
                PostedDate = row.Date,
                Amount = row.Amount,
                Description = row.Concept,
                CurrencyId = row.CurrencyId,
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

