using CQRSDispatch;
using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using Finance.Application.Specifications.CreditCards;
using Finance.Application.Repositories;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;

namespace Finance.Application.Commands.CreditCards;

public class UpdateCreditCardStatementImportTemplateCommandHandler(
    IRepository<CreditCardStatementImportTemplate, Guid> repository,
    FinanceDbContext db,
    CanSetSystemFlag canSetSystemFlag)
    : BaseUpdateCommandHandler<CreditCardStatementImportTemplate, Guid, UpdateCreditCardStatementImportTemplateCommand>(
        repository, db)
{
    public override async Task<DataResult<CreditCardStatementImportTemplate>> ExecuteAsync(
        UpdateCreditCardStatementImportTemplateCommand command, CancellationToken cancellationToken = default)
    {
        if (command.IsSystem)
        {
            var check = await canSetSystemFlag.IsSatisfiedAsync(cancellationToken);
            if (!check.IsSuccess)
                return DataResult<CreditCardStatementImportTemplate>.Failure(check.ErrorMessage!);
        }
        return await base.ExecuteAsync(command, cancellationToken);
    }

    protected override Task<CreditCardStatementImportTemplate> UpdateRecord(
        UpdateCreditCardStatementImportTemplateCommand command,
        CreditCardStatementImportTemplate record,
        CancellationToken cancellationToken)
    {
        record.Name = command.Name;
        record.ConfigJson = command.ConfigJson;
        record.IsSystem = command.IsSystem;
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
