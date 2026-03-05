namespace Finance.Api.Controllers.Requests;

public class UpdateUserRequest : BaseUserRequest
{
    public Guid Id { get; set; }
}
