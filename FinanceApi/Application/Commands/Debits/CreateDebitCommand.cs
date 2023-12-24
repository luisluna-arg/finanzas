using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.Debits;

public class CreateDebitCommandHandler : BaseResponseHandler<CreateDebitCommand, Debit>
{
    private readonly IRepository<Debit, Guid> debitOriginRepository;

    public CreateDebitCommandHandler(
        FinanceDbContext db,
        IRepository<Debit, Guid> debitRepository)
        : base(db)
    {
        this.debitOriginRepository = debitRepository;
    }

    public override async Task<Debit> Handle(CreateDebitCommand command, CancellationToken cancellationToken)
    {
        var newDebit = new Debit()
        {
            OriginId = command.OriginId,
            Amount = command.Amount,
            TimeStamp = DateTime.UtcNow
        };

        await debitOriginRepository.Add(newDebit);

        return await Task.FromResult(newDebit);
    }
}

public class CreateDebitCommand : IRequest<Debit>
{
    [Required]
    public Guid OriginId { get; set; }

    [Required]
    public Money Amount { get; set; } = 0m;

    [Required]
    public bool Deactivated { get; set; }
}
