using MediatR;

namespace FinanceApi.Application.Commands.IOLInvestments;

public class UploadIOLInvestmentsCommand : IRequest
{
    public UploadIOLInvestmentsCommand(IFormFile file)
    {
        this.File = file;
    }

    public IFormFile File { get; set; }
}
