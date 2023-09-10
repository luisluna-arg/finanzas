using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.Currencies;

public class CreateCurrencyCommandHandler : BaseResponseHandler<CreateCurrencyCommand, Currency>
{
    private readonly IRepository<Currency, Guid> currencyRepository;

    public CreateCurrencyCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository)
        : base(db)
    {
        this.currencyRepository = currencyRepository;
    }

    public override async Task<Currency> Handle(CreateCurrencyCommand command, CancellationToken cancellationToken)
    {
        var newCurrency = new Currency()
        {
            ShortName = command.ShortName,
            Name = command.Name
        };

        await currencyRepository.Add(newCurrency);

        return await Task.FromResult(newCurrency);
    }
}

public class CreateCurrencyCommand : IRequest<Currency>
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string ShortName { get; set; } = string.Empty;
}

