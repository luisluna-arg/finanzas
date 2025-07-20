using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.Banks;
using Finance.Application.Mapping;
using Finance.Application.Queries.Banks;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/banks")]
public class BankQueryController(IMappingService mapper, IDispatcher dispatcher)
    : BasicQueryController<Bank, Guid, BankDto, GetAllBanksQuery, GetBankQuery>(mapper, dispatcher);
