using MediatR;

namespace FinanceApi.Application.Commands.Funds;

public class UploadFundFileCommand : IRequest
{
    required public IFormFileCollection Files { get; set; }

    required public DateTimeKind DateKind { get; set; }
}
