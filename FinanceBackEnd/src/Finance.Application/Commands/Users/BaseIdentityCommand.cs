using System.Text.Json.Serialization;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Enums;
using Finance.Domain.Models.Identities;

namespace Finance.Application.Commands;

public abstract class BaseIdentityCommand : ICommand<DataResult<Identity>>
{
    public Guid UserId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public IdentityProviderEnum Provider { get; set; }
    public string SourceId { get; set; } = string.Empty;
}
