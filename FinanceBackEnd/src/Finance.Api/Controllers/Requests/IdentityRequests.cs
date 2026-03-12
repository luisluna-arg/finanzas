using System.Text.Json.Serialization;
using Finance.Domain.Enums;

namespace Finance.Api.Controllers.Requests;

public class CreateIdentityRequest
{
    public Guid UserId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public IdentityProviderEnum Provider { get; set; }
    public string SourceId { get; set; } = string.Empty;
}

public sealed class UpdateIdentityRequest : CreateIdentityRequest
{
    public Guid IdentityId { get; set; }
}

public sealed class DeleteIdentityRequest
{
    public Guid UserId { get; set; }
    public Guid IdentityId { get; set; }
}
