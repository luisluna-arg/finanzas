using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;

namespace Finance.Application.Commands.CreditCards;

public class UpdateCreditCardStatementImportTemplateCommandHandler(
    IRepository<CreditCardStatementImportTemplate, Guid> repository,
    FinanceDbContext db)
    : BaseUpdateCommandHandler<CreditCardStatementImportTemplate, Guid, UpdateCreditCardStatementImportTemplateCommand>(
        repository, db)
{
    protected override Task<CreditCardStatementImportTemplate> UpdateRecord(
        UpdateCreditCardStatementImportTemplateCommand command,
        CreditCardStatementImportTemplate record,
        CancellationToken cancellationToken)
    {
        record.Name = command.Name;
        record.IsSystem = command.IsSystem;
        record.ConfigJson = command.ConfigJson;
        return Task.FromResult(record);
    }
}

public class UpdateCreditCardStatementImportTemplateCommand
    : BaseUpdateCommand<CreditCardStatementImportTemplate, Guid>
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public bool IsSystem { get; set; }

    [Required]
    public string ConfigJson { get; set; } = string.Empty;
}
