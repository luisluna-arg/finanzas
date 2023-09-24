using AutoMapper;
using FinanceApi.Application.Commands.IOLInvestments;
using FinanceApi.Application.Dtos.IOLInvestmentAssetTypes;
using FinanceApi.Application.Queries.IOLInvestmentAssetTypes;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/iol-investment-asset-type")]
public class IOLInvestmentAssetTypeController : ApiBaseController<IOLInvestmentAssetType?, ushort, IOLInvestmentAssetTypeDto>
{
    public IOLInvestmentAssetTypeController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetTypes()
    => await Handle(new GetAllIOLInvestmentAssetTypesQuery());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTypeById(ushort id)
        => await Handle(new GetIOLInvestmentAssetTypeQuery { Id = id });

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(ushort id)
        => await Handle(new ActivateIOLInvestmentAssetTypeCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(ushort id)
        => await Handle404(new DeactivateIOLInvestmentAssetTypeCommand { Id = id });
}
