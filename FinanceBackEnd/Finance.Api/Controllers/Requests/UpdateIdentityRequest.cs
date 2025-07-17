using Finance.Domain.Enums;

namespace Finance.Api.Controllers.Requests;

public class UpdateIdentityRequest
{
    public Guid UserId { get; set; }
    public Guid IdentityId { get; set; }
    public IdentityProviderEnum Provider { get; set; }
    public string SourceId { get; set; } = string.Empty;
}
