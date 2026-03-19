using System.ComponentModel;
using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Commands.Debits;
using Finance.Application.Dtos.Debits;
using Finance.Application.Helpers;
using Finance.Application.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.Debits;
using Finance.Domain.Enums;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

public abstract class DebitCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    DebitService debitService)
    : CommandController<
        Debit,
        DebitPermissions,
        CreateDebitRequest,
        UpdateDebitRequest,
        DeleteDebitRequest,
        SetDebitOwnerRequest,
        DeleteDebitOwnerRequest,
        Guid,
        DebitDto,
        DebitService>(mapper, dispatcher, debitService)
{
    [HttpPost]
    [Route("upload")]
    protected async Task<IActionResult> Upload(IFormFile file, string appModuleId, [DefaultValue("Local")] string dateKind, FrequencyEnum frequency)
    {
        await Dispatcher.DispatchCommandAsync(new UploadDebitsFileCommand(file, appModuleId, EnumHelper.Parse<DateTimeKind>(dateKind), frequency));
        return Ok();
    }
}
