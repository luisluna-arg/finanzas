using CQRSDispatch;
using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Application.Specifications.CreditCards;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;

namespace Finance.Application.Commands.CreditCards;

public class UpdateCreditCardInstallmentPatternCommandHandler(
    IRepository<CreditCardInstallmentPattern, Guid> repository,
    FinanceDbContext db,
    CanSetSystemFlag canSetSystemFlag)
    : BaseUpdateCommandHandler<CreditCardInstallmentPattern, Guid, UpdateCreditCardInstallmentPatternCommand>(
        repository, db)
{
    public override async Task<DataResult<CreditCardInstallmentPattern>> ExecuteAsync(
        UpdateCreditCardInstallmentPatternCommand command, CancellationToken cancellationToken = default)
    {
        if (command.IsSystem)
        {
            var check = await canSetSystemFlag.IsSatisfiedAsync(cancellationToken);
            if (!check.IsSuccess)
                return DataResult<CreditCardInstallmentPattern>.Failure(check.ErrorMessage!);
        }
        return await base.ExecuteAsync(command, cancellationToken);
    }

    protected override Task<CreditCardInstallmentPattern> UpdateRecord(
        UpdateCreditCardInstallmentPatternCommand command,
        CreditCardInstallmentPattern record,
        CancellationToken cancellationToken)
    {
        record.Name = command.Name;
        record.RegexPattern = command.RegexPattern;
        record.IsSystem = command.IsSystem;
        return Task.FromResult(record);
    }
}

public class UpdateCreditCardInstallmentPatternCommand
    : BaseUpdateCommand<CreditCardInstallmentPattern, Guid>
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string RegexPattern { get; set; } = string.Empty;

    public bool IsSystem { get; set; }
}
