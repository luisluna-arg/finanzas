using FinanceApi.Application.Commands.Currencies;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.Currencies;

public class DeleteCurrencyCommandHandler : BaseResponselessHandler<DeleteCurrencyCommand>
{
    private readonly IRepository<Currency, Guid> _currencyRepository;

    public DeleteCurrencyCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository)
        : base(db)
    {
        _currencyRepository = currencyRepository;
    }

    public override async Task Handle(DeleteCurrencyCommand command, CancellationToken cancellationToken)
    {
        var currency = await _currencyRepository.GetById(command.Id);
        DbContext.Remove(currency);
        await DbContext.SaveChangesAsync();
    }
}
