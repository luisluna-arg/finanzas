
using Finance.Application.Commands.Base;
using Finance.Domain.Enums;
using Finance.Domain.Models.IOLInvestments;
using Finance.Persistence;

namespace Finance.Application.Commands.IOLInvestments;

public class ActivateIOLInvestmentAssetTypeCommand : BatchUpdateBaseCommand<IOLInvestmentAssetTypeEnum>;

public class ActivateIOLInvestmentAssetTypeCommandHandler(FinanceDbContext dbContext)
    : BaseActivateCommandHandler<ActivateIOLInvestmentAssetTypeCommand, ActivateIOLInvestmentAssetTypeCommandValidator, IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum>(dbContext);

public class ActivateIOLInvestmentAssetTypeCommandValidator : BatchUpdateBaseCommandValidator<ActivateIOLInvestmentAssetTypeCommand, IOLInvestmentAssetTypeEnum>;
