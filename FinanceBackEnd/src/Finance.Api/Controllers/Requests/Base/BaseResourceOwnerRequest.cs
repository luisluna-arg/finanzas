namespace Finance.Api.Controllers.Requests.Base;

public abstract class BaseResourceOwnerRequest
{
    protected BaseResourceOwnerRequest(Guid resourceId, Guid userId)
    {
        ResourceId = resourceId;
        UserId = userId;
    }

    public Guid ResourceId { get; set; }
    public Guid UserId { get; set; }
}
