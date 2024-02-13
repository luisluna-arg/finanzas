using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.CurrencyExchangeRates;

public class DeleteCurrencyExchangeRateCommandHandler : BaseResponselessHandler<DeleteCurrencyExchangeRateCommand>
{
    private readonly IRepository<CurrencyExchangeRate, Guid> currencyRepository;

    public DeleteCurrencyExchangeRateCommandHandler(
        FinanceDbContext db,
        IRepository<CurrencyExchangeRate, Guid> currencyRepository)
        : base(db)
    {
        this.currencyRepository = currencyRepository;
    }

    public override async Task Handle(DeleteCurrencyExchangeRateCommand command, CancellationToken cancellationToken)
        => await currencyRepository.Delete(command.Id);
}

public class DeleteCurrencyExchangeRateCommand : IRequest
{
    [Required]
    public Guid Id { get; set; }
}
