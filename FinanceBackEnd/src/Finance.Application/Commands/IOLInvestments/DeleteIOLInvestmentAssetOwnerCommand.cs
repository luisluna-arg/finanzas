using Finance.Application.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.IOLInvestments;
using Finance.Persistence;

namespace Finance.Application.Commands.IOLInvestments;

public sealed class DeleteIOLInvestmentAssetOwnerCommand
    : DeleteEntityOwnerCommand<IOLInvestmentAsset, Guid, IOLInvestmentAssetPermissions>;

public sealed class DeleteIOLInvestmentAssetOwnerCommandHandler(FinanceDbContext dbContext)
    : DeleteEntityOwnerCommandHandler<DeleteIOLInvestmentAssetOwnerCommand, IOLInvestmentAsset, Guid, IOLInvestmentAssetPermissions>(dbContext);
