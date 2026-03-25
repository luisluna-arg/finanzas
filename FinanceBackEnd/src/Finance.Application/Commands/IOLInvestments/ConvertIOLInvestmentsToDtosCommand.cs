using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Dtos.IOLInvestments;
using Finance.Application.Mapping;
using Finance.Application.Services;
using Finance.Domain.Models.Interfaces;
using Finance.Domain.Models.IOLInvestments;
using Finance.Domain.SpecialTypes;
using Finance.Persistence.Constants;

namespace Finance.Application.Commands.IOLInvestments;

public class ConvertIOLInvestmentsToDtosCommand : ICommand<DataResult<ICollection<IOLInvestmentDto>>>
{
    public required ICollection<IOLInvestment> Investments { get; set; }
}

public class ConvertIOLInvestmentsToDtosCommandHandler(
    CurrencyConversionService currencyConverter,
    IMappingService mappingService)
    : ICommandHandler<ConvertIOLInvestmentsToDtosCommand, DataResult<ICollection<IOLInvestmentDto>>>
{
    public async Task<DataResult<ICollection<IOLInvestmentDto>>> ExecuteAsync(
        ConvertIOLInvestmentsToDtosCommand command, CancellationToken cancellationToken = default)
    {
        var investments = command.Investments;
        var defaultCurrencyId = Guid.Parse(CurrencyConstants.DefaultCurrencyId);

        var holders = investments
            .Select(o => (IAmountHolder)new InvestmentAmountHolder { CurrencyId = o.Asset.CurrencyId, Amount = o.Valued })
            .ToList();
        var convertedValues = (await currencyConverter.ConvertCollection(holders, defaultCurrencyId)).ToArray();

        var dtos = investments.Select((o, i) =>
        {
            var dto = mappingService.Map<IOLInvestmentDto>(o);
            dto.Valued = holders[i].Amount;
            dto.ValuedConversion = convertedValues[i];
            return dto;
        }).ToList();

        return DataResult<ICollection<IOLInvestmentDto>>.Success(dtos);
    }

    private sealed class InvestmentAmountHolder : IAmountHolder
    {
        public Guid CurrencyId { get; set; }
        public Money Amount { get; set; }
    }
}
