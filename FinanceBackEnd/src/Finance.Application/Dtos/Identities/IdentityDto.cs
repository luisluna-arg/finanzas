using Finance.Application.Dtos.Base;
using Finance.Domain.Enums;

namespace Finance.Application.Dtos.Identities;

public record IdentityDto : Dto<Guid>
{
    public IdentityProviderEnum Provider { get; set; } = default;
    public string SourceId { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public IdentityDto()
    {
    }
}
