using Finance.Application.Base.Handlers;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;

namespace Finance.Application.Commands.CurrencyConvertions;

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
        => await currencyConversionRepository.DeleteAsync(command.Id, cancellationToken);
}

public class DeleteCurrencyConversionCommand : IRequest
{
    required public Guid Id { get; set; }
}
