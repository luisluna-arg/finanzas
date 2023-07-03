using MediatR;

namespace FinanceApi.Application.Commands.Funds;

public class ProcessImageToTextCommand : IRequest
{
    required public HttpContext HttpContext { get; set; }

    required public IFormFileCollection Files { get; set; }

    public DateTimeKind? DateKind { get; set; }

    public DateTime? DateReference { get; internal set; }
}
