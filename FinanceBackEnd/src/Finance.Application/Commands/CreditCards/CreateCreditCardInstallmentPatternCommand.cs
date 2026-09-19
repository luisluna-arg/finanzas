using CQRSDispatch;
using System.ComponentModel.DataAnnotations;
using Finance.Application.Auth;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Application.Specifications.CreditCards;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;

namespace Finance.Application.Commands.CreditCards;

public class CreateCreditCardInstallmentPatternCommandHandler
    : BaseCreateCommandHandler<CreateCreditCardInstallmentPatternCommand, CreditCardInstallmentPattern, Guid>
{
    private readonly IsAdminUser _isAdminUser;
    private readonly CanSetSystemFlag _canSetSystemFlag;

    public CreateCreditCardInstallmentPatternCommandHandler(
        IRepository<CreditCardInstallmentPattern, Guid> repository,
        FinanceDbContext db,
        IsAdminUser isAdminUser,
        CanSetSystemFlag canSetSystemFlag)
        : base(repository, db)
    {
        _isAdminUser = isAdminUser;
        _canSetSystemFlag = canSetSystemFlag;
    }

    public override async Task<DataResult<CreditCardInstallmentPattern>> ExecuteAsync(
        CreateCreditCardInstallmentPatternCommand command, CancellationToken cancellationToken = default)
    {
        if (command.IsSystem)
        {
            var check = await _canSetSystemFlag.IsSatisfiedAsync(cancellationToken);
            if (!check.IsSuccess)
                return DataResult<CreditCardInstallmentPattern>.Failure(check.ErrorMessage!);
        }
        return await base.ExecuteAsync(command, cancellationToken);
    }

    protected override async Task<CreditCardInstallmentPattern> BuildRecord(
        CreateCreditCardInstallmentPatternCommand command, CancellationToken cancellationToken)
    {
        var (_, userId) = await _isAdminUser.IsSatisfiedAsync(cancellationToken);

        return new CreditCardInstallmentPattern
        {
            Name = command.Name,
            RegexPattern = command.RegexPattern,
            IsSystem = command.IsSystem,
            UserId = command.IsSystem ? null : userId,
        };
    }
}

public class CreateCreditCardInstallmentPatternCommand : BaseCreateCommand<CreditCardInstallmentPattern>
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string RegexPattern { get; set; } = string.Empty;

    public bool IsSystem { get; set; }
}
