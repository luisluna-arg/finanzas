namespace Finance.Api.Controllers.Requests;

public sealed class CreateUserRequest : BaseUserRequest;

public sealed class UpdateUserRequest : BaseUserRequest
{
    public Guid Id { get; set; }
}

public sealed class DeleteUserRequest
{
    public Guid Id { get; set; }
}
