using AutoMapper;
using FinanceApi.Application.Commands.CreditCards;
using FinanceApi.Application.Dtos.CreditCards;
using FinanceApi.Application.Queries.CreditCardIssuers;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/credit-card-issuers")]
public class CreditCardIssuerController : ApiBaseController<CreditCardIssuer?, Guid, CreditCardIssuerDto>
{
    public CreditCardIssuerController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetCreditCardIssuersQuery query)
        => await Handle(query);

    [HttpPost]
    public async Task<IActionResult> Create(CreateCreditCardIssuerCommand command)
    => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCreditCardIssuerCommand command)
        => await Handle(command);

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await Handle(new ActivateCreditCardIssuerCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404(new DeactivateCreditCardIssuerCommand { Id = id });
}
