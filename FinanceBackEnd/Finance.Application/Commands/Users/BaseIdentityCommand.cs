using Finance.Domain.Models;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Enums;
using System.Text.Json.Serialization;

namespace Finance.Application.Commands;

public abstract class BaseIdentityCommand : ICommand<DataResult<Identity>>
{
    public Guid UserId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public IdentityProviderEnum Provider { get; set; }
    public string SourceId { get; set; } = string.Empty;
}
