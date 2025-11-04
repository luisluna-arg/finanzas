namespace Finance.Api.Controllers.Requests.Base;

public abstract class BaseFundOwnerRequest
{
    protected BaseFundOwnerRequest(Guid fundId, Guid userId)
    {
        FundId = fundId;
        UserId = userId;
    }

    public Guid FundId { get; set; }
    public Guid UserId { get; set; }
}
