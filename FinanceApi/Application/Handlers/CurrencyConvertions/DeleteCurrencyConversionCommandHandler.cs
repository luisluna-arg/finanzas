using FinanceApi.Application.Commands.CurrencyConversions;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.CurrencyConvertions;

public class DeleteCurrencyConversionCommandHandler : BaseResponselessHandler<DeleteCurrencyConversionCommand>
{
    private readonly IRepository<CurrencyConversion, Guid> currencyConversionRepository;

    public DeleteCurrencyConversionCommandHandler(
        FinanceDbContext db,
        IRepository<CurrencyConversion, Guid> currencyConversionRepository)
        : base(db)
    {
        this.currencyConversionRepository = currencyConversionRepository;
    }

    public override async Task Handle(DeleteCurrencyConversionCommand command, CancellationToken cancellationToken)
        => await currencyConversionRepository.Delete(command.Id);
}
