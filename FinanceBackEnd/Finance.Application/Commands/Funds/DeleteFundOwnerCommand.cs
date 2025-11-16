
using Finance.Application.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Funds;
using Finance.Persistence;

namespace Finance.Application.Commands;

public class DeleteFundOwnerCommand
    : DeleteEntityOwnerCommand<Fund, Guid, FundPermissions>;

public class DeleteFundOwnerCommandHandler(FinanceDbContext dbContext)
    : DeleteEntityOwnerCommandHandler<DeleteFundOwnerCommand, Fund, Guid, FundPermissions>(dbContext);
