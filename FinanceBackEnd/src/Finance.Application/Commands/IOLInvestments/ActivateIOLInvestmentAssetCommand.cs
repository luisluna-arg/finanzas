using Finance.Application.Commands.Base;
using Finance.Domain.Models.IOLInvestments;
using Finance.Persistence;

namespace Finance.Application.Commands.IOLInvestments;

public class ActivateIOLInvestmentAssetCommand : BatchUpdateBaseCommand;

public class ActivateIOLInvestmentAssetCommandHandler(FinanceDbContext dbContext)
    : BaseActivateCommandHandler<ActivateIOLInvestmentAssetCommand, ActivateIOLInvestmentAssetCommandValidator, IOLInvestmentAsset, Guid>(dbContext);

public class ActivateIOLInvestmentAssetCommandValidator : BatchUpdateBaseCommandValidator<ActivateIOLInvestmentAssetCommand>;
