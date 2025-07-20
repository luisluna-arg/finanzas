using System.Text.Json.Serialization;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Enums;
using Finance.Domain.Models;

namespace Finance.Application.Commands.Users;

public abstract class BaseUserCommand : ICommand<DataResult<User>>
{
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public IEnumerable<RoleEnum> Roles { get; set; } = [];
}