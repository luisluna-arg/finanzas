using System.Text.Json.Serialization;
using Finance.Application.Dtos.Identities;
using Finance.Domain.Enums;

namespace Finance.Api.Controllers.Requests;

public abstract class BaseUserRequest
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public IEnumerable<RoleEnum> Roles { get; set; }
    public IEnumerable<IdentityDto> Identities { get; set; } = [];
}