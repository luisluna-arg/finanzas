using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Commands.DebitOrigins;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.Debits;

public class CreateDebitCommandHandler : BaseResponseHandler<CreateDebitCommand, Debit>
{
    private readonly IMediator mediator;

    private readonly IRepository<Debit, Guid> debitRepository;

    private readonly IRepository<DebitOrigin, Guid> debitOriginRepository;

    public CreateDebitCommandHandler(
        FinanceDbContext db,
        IMediator mediator,
        IRepository<Debit, Guid> debitRepository,
        IRepository<DebitOrigin, Guid> debitOriginRepository)
        : base(db)
    {
        this.mediator = mediator;
        this.debitRepository = debitRepository;
        this.debitOriginRepository = debitOriginRepository;
    }

    public override async Task<Debit> Handle(CreateDebitCommand command, CancellationToken cancellationToken)
    {
        var originName = command.Origin.Trim();

        var origin = await debitOriginRepository.GetByAsync(
            new Dictionary<string, object>()
            {
                { "Name", originName },
                { "AppModuleId", command.AppModuleId }
            },
            cancellationToken);

        if (origin == null)
        {
            var createDebitOriginRequest = new CreateDebitOriginCommand()
            {
                Name = originName,
                AppModuleId = command.AppModuleId
            };

            origin = await mediator.Send(createDebitOriginRequest)!;
        }

        var newDebit = new Debit()
        {
            Origin = origin!,
            Amount = command.Amount,
            TimeStamp = DateTime.UtcNow
        };

        await debitRepository.AddAsync(newDebit, cancellationToken);

        return await Task.FromResult(newDebit);
    }
}

public class CreateDebitCommand : IRequest<Debit>
{
    public Guid AppModuleId { get; set; }

    [Required]
    public string Origin { get; set; }

    [Required]
    public decimal Amount { get; set; } = 0m;

    [Required]
    public bool Deactivated { get; set; }
}
