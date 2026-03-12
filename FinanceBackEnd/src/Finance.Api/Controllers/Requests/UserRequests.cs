namespace Finance.Api.Controllers.Requests;

public class CreateUserRequest : BaseUserRequest;

public sealed class UpdateUserRequest : CreateUserRequest
{
    public Guid Id { get; set; }
}

public sealed class DeleteUserRequest
{
    public Guid Id { get; set; }
}
