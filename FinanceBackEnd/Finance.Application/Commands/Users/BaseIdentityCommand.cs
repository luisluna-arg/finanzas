using Finance.Domain.Models;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Enums;

namespace Finance.Application.Commands;

public abstract class BaseIdentityCommand : ICommand<DataResult<Identity>>
{
    public Guid UserId { get; set; }
    public IdentityProviderEnum Provider { get; set; }
    public string SourceId { get; set; } = string.Empty;
}
