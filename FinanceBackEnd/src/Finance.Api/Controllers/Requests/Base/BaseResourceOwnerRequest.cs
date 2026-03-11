namespace Finance.Api.Controllers.Requests.Base;

public abstract class BaseResourceOwnerRequest<TResourceId>
    where TResourceId : struct
{
    protected BaseResourceOwnerRequest(TResourceId resourceId, Guid userId)
    {
        ResourceId = resourceId;
        UserId = userId;
    }

    public TResourceId ResourceId { get; set; }
    public Guid UserId { get; set; }
}

public abstract class BaseResourceOwnerRequest : BaseResourceOwnerRequest<Guid>
{
    protected BaseResourceOwnerRequest(Guid resourceId, Guid userId)
        : base(resourceId, userId)
    {
    }
}
