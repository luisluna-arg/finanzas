using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.Incomes;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.Incomes;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Incomes;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/incomes")]
public class IncomeCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    IncomeService incomeService)
    : CommandController<
        Income,
        IncomePermissions,
        CreateIncomeRequest,
        UpdateIncomeRequest,
        DeleteIncomeRequest,
        SetIncomeOwnerRequest,
        DeleteIncomeOwnerRequest,
        Guid,
        IncomeDto,
        IncomeService>(mapper, dispatcher, incomeService)
{
}
