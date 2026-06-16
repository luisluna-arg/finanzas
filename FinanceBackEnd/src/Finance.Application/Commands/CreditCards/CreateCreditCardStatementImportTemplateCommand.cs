using CQRSDispatch;
using System.ComponentModel.DataAnnotations;
using Finance.Application.Auth;
using Finance.Application.Base.Handlers;
using Finance.Application.Specifications.CreditCards;
using Finance.Application.Repositories;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.CreditCards;

public class CreateCreditCardStatementImportTemplateCommandHandler
    : BaseCreateCommandHandler<CreateCreditCardStatementImportTemplateCommand, CreditCardStatementImportTemplate, Guid>
{
    private readonly FinanceDbContext _db;
    private readonly IsAdminUser _isAdminUser;
    private readonly CanSetSystemFlag _canSetSystemFlag;

    public CreateCreditCardStatementImportTemplateCommandHandler(
        IRepository<CreditCardStatementImportTemplate, Guid> repository,
        FinanceDbContext db,
        IsAdminUser isAdminUser,
        CanSetSystemFlag canSetSystemFlag)
        : base(repository, db)
    {
        _db = db;
        _isAdminUser = isAdminUser;
        _canSetSystemFlag = canSetSystemFlag;
    }

    public override async Task<DataResult<CreditCardStatementImportTemplate>> ExecuteAsync(
        CreateCreditCardStatementImportTemplateCommand command, CancellationToken cancellationToken = default)
    {
        if (command.IsSystem)
        {
            var check = await _canSetSystemFlag.IsSatisfiedAsync(cancellationToken);
            if (!check.IsSuccess)
                return DataResult<CreditCardStatementImportTemplate>.Failure(check.ErrorMessage!);
        }
        return await base.ExecuteAsync(command, cancellationToken);
    }

    protected override async Task<CreditCardStatementImportTemplate> BuildRecord(
        CreateCreditCardStatementImportTemplateCommand command, CancellationToken cancellationToken)
    {
        var (_, userId) = await _isAdminUser.IsSatisfiedAsync(cancellationToken);

        var template = new CreditCardStatementImportTemplate
        {
            Name = command.Name,
            IsSystem = command.IsSystem,
            UserId = userId,
            ConfigJson = command.ConfigJson,
        };

        if (command.CreditCardId.HasValue)
        {
            var creditCard = await _db.CreditCard
                .FirstOrDefaultAsync(c => c.Id == command.CreditCardId.Value, cancellationToken);
            if (creditCard != null)
                template.CreditCards.Add(creditCard);
        }

        return template;
    }
}

public class CreateCreditCardStatementImportTemplateCommand : BaseCreateCommand<CreditCardStatementImportTemplate>
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public bool IsSystem { get; set; }

    [Required]
    public string ConfigJson { get; set; } = string.Empty;

    public Guid? CreditCardId { get; set; }
}
