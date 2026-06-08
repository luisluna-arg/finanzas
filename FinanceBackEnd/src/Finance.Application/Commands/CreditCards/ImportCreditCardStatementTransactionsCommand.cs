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

        var config = JsonSerializer.Deserialize<StatementImportConfig>(template.ConfigJson)
            ?? throw new Exception("Invalid template configuration");

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
            await DbContext.SaveChangesAsync(cancellationToken);

            var statementTx = new CreditCardStatementTransaction
            {
                StatementId = statement.Id,
                CreditCardTransactionId = tx.Id,
                PostedDate = row.Date,
                Amount = row.Amount,
                Description = row.Concept,
                CurrencyId = config.DefaultCurrencyId,
            };
            DbContext.CreditCardStatementTransaction.Add(statementTx);
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

public class StatementImportConfig
{
    public int SkipRows { get; set; } = 1;
    public int DateColumn { get; set; } = 0;
    public string DateFormat { get; set; } = "d/M/yyyy";
    public int ConceptColumn { get; set; } = 1;
    public int AmountColumn { get; set; } = 2;
    public Guid DefaultCurrencyId { get; set; }
    public string DecimalSeparator { get; set; } = ",";
    public string ThousandsSeparator { get; set; } = ".";
    public bool AmountNegate { get; set; } = false;
}
