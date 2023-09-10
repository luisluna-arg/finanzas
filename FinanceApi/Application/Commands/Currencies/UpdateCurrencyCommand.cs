using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.Currencies;

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
        var currency = await currencyRepository.GetById(command.Id);
        if (currency == null) throw new Exception("Currency not found");

        currency.Name = command.Name;
        currency.ShortName = command.ShortName;

        await currencyRepository.Update(currency);

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
