using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.CreditCards;

public class UpdateCreditCardIssuerCommandHandler : BaseResponseHandler<UpdateCreditCardIssuerCommand, CreditCardIssuer>
{
    private readonly IRepository<CreditCardIssuer, Guid> creditCardIssuerRepository;
    private readonly IRepository<Bank, Guid> bankRepository;

    public UpdateCreditCardIssuerCommandHandler(
        FinanceDbContext db,
        IRepository<Bank, Guid> bankRepository,
        IRepository<CreditCardIssuer, Guid> creditCardIssuerRepository)
        : base(db)
    {
        this.bankRepository = bankRepository;
        this.creditCardIssuerRepository = creditCardIssuerRepository;
    }

    public override async Task<CreditCardIssuer> Handle(UpdateCreditCardIssuerCommand command, CancellationToken cancellationToken)
    {
        var bank = await bankRepository.GetById(command.BankId);
        if (bank == null) throw new Exception("Bank not found");

        var creditCardIssuer = await creditCardIssuerRepository.GetById(command.Id);
        if (creditCardIssuer == null) throw new Exception("Credit Card Issuer not found");

        creditCardIssuer.Bank = bank;
        creditCardIssuer.Name = command.Name;

        await creditCardIssuerRepository.Update(creditCardIssuer);
        await creditCardIssuerRepository.Persist();

        return await Task.FromResult(creditCardIssuer);
    }
}

public class UpdateCreditCardIssuerCommand : IRequest<CreditCardIssuer>
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public Guid BankId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
}
