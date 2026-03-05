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
