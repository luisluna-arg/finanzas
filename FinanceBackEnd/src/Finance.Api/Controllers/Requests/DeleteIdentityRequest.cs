namespace Finance.Api.Controllers.Requests;

public class DeleteIdentityRequest
{
    public Guid UserId { get; set; }
    public Guid IdentityId { get; set; }
}
