
using Finance.Application.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Funds;
using Finance.Persistence;

namespace Finance.Application.Commands;

public sealed class DeleteFundOwnerCommand
    : DeleteEntityOwnerCommand<Fund, Guid, FundPermissions>;

public sealed class DeleteFundOwnerCommandHandler(FinanceDbContext dbContext)
    : DeleteEntityOwnerCommandHandler<DeleteFundOwnerCommand, Fund, Guid, FundPermissions>(dbContext);
