using Finance.Application.Commands.Base;
using Finance.Domain.Models.IOLInvestments;
using Finance.Persistence;

namespace Finance.Application.Commands.IOLInvestments;

public class DeactivateIOLInvestmentAssetCommand : BatchUpdateBaseCommand;

public class DeactivateIOLInvestmentAssetCommandHandler(FinanceDbContext dbContext)
    : BaseDeactivateCommandHandler<DeactivateIOLInvestmentAssetCommand, DeactivateIOLInvestmentAssetCommandValidator, IOLInvestmentAsset, Guid>(dbContext);

public class DeactivateIOLInvestmentAssetCommandValidator : BatchUpdateBaseCommandValidator<DeactivateIOLInvestmentAssetCommand>;
