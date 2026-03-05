using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.Banks;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Legacy.Queries.Banks;
using Finance.Domain.Models.Banks;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/banks")]
public class BankQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : BasicQueryController<Bank, Guid, BankDto, GetAllBanksQuery, GetBankQuery>(mapper, dispatcher);
