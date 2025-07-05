using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.Banks;
using Finance.Application.Queries.Banks;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/banks")]
public class BankQueryController(IMapper mapper, IMediator mediator)
    : BasicQueryController<Bank?, Guid, BankDto, GetAllBanksQuery, GetBankQuery>(mapper, mediator);
