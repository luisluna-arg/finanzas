namespace Finance.Api.Controllers.Requests;

public sealed class DeleteIdentityRequest
{
    public Guid UserId { get; set; }
    public Guid IdentityId { get; set; }
}
