using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.CreditCards;

public class CreateCreditCardStatementImportTemplateCommandHandler
    : BaseCreateCommandHandler<CreateCreditCardStatementImportTemplateCommand, CreditCardStatementImportTemplate, Guid>
{
    private readonly FinanceDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateCreditCardStatementImportTemplateCommandHandler(
        IRepository<CreditCardStatementImportTemplate, Guid> repository,
        FinanceDbContext db,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, db)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<CreditCardStatementImportTemplate> BuildRecord(
        CreateCreditCardStatementImportTemplateCommand command, CancellationToken cancellationToken)
    {
        Guid? userId = null;
        if (!command.IsSystem)
        {
            var identitySourceId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(identitySourceId))
            {
                var user = await _db.User
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .Include(u => u.Identities)
                    .FirstOrDefaultAsync(u => u.Identities.Any(i => i.SourceId == identitySourceId), cancellationToken);
                userId = user?.Id;
            }
        }

        return new CreditCardStatementImportTemplate
        {
            Name = command.Name,
            IsSystem = command.IsSystem,
            UserId = userId,
            ConfigJson = command.ConfigJson,
        };
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
}
