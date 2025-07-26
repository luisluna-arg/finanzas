using System.ComponentModel;
using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Commands.Debits;
using Finance.Application.Dtos.Debits;
using Finance.Application.Helpers;
using Finance.Application.Mapping;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

public abstract class DebitCommandController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCommandController<Debit?, Guid, DebitDto>(mapper, dispatcher)
{
    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteDebitCommand request)
        => await ExecuteAsync(request);

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await ExecuteAsync(new ActivateDebitCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404Nullable(new DeactivateDebitCommand { Id = id });

    [HttpPost]
    protected async Task<IActionResult> Create(CreateDebitCommand command)
        => await ExecuteAsync(command);

    [HttpPost]
    [Route("upload")]
    protected async Task<IActionResult> Upload(IFormFile file, string appModuleId, [DefaultValue("Local")] string dateKind, FrequencyEnum frequency)
    {
        await ExecuteAsync(new UploadDebitsFileCommand(file, appModuleId, EnumHelper.Parse<DateTimeKind>(dateKind), frequency));
        return Ok();
    }
}
