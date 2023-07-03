using MediatR;

namespace FinanceApi.Application.Commands.Funds;

public class UploadImageCommand : IRequest
{
    required public IFormFileCollection Files { get; set; }

    public DateTimeKind? DateKind { get; set; }

    public DateTime? DateReference { get; internal set; }
}
