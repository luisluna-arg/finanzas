using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.CurrencyConvertions;

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

public class DeleteCurrencyConversionCommand : IRequest
{
    required public Guid Id { get; set; }
}
