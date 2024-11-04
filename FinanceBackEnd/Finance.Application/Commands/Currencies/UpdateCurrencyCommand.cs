using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;

namespace Finance.Application.Commands.Currencies;

public class UpdateCurrencyCommandHandler : BaseResponseHandler<UpdateCurrencyCommand, Currency>
{
    private readonly IRepository<Currency, Guid> currencyRepository;

    public UpdateCurrencyCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository)
        : base(db)
    {
        this.currencyRepository = currencyRepository;
    }

    public override async Task<Currency> Handle(UpdateCurrencyCommand command, CancellationToken cancellationToken)
    {
        var currency = await currencyRepository.GetByIdAsync(command.Id, cancellationToken);
        if (currency == null) throw new Exception("Currency not found");

        currency.Name = command.Name;
        currency.ShortName = command.ShortName;

        await currencyRepository.UpdateAsync(currency, cancellationToken);

        return await Task.FromResult(currency);
    }
}

public class UpdateCurrencyCommand : IRequest<Currency>
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string ShortName { get; set; } = string.Empty;
}
