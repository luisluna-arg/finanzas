using System.Text.Json.Serialization;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Domain.Enums;
using Finance.Domain.Models;

namespace Finance.Application.Commands.Users;

public abstract class BaseUserCommand : IContextAwareCommand<FinanceDispatchContext, DataResult<User>>
{
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public IEnumerable<RoleEnum> Roles { get; set; } = [];
    public FinanceDispatchContext Context { get; private set; } = new();

    public void SetContext(FinanceDispatchContext context)
    {
        Context = context;
    }
}