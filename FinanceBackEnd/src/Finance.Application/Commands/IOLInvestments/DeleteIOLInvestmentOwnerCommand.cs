using Finance.Application.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.IOLInvestments;
using Finance.Persistence;

namespace Finance.Application.Commands.IOLInvestments;

public sealed class DeleteIOLInvestmentOwnerCommand
    : DeleteEntityOwnerCommand<IOLInvestment, Guid, IOLInvestmentPermissions>;

public sealed class DeleteIOLInvestmentOwnerCommandHandler(FinanceDbContext dbContext)
    : DeleteEntityOwnerCommandHandler<DeleteIOLInvestmentOwnerCommand, IOLInvestment, Guid, IOLInvestmentPermissions>(dbContext);
