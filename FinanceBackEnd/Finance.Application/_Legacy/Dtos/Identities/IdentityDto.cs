using Finance.Application.Legacy.Dtos.Base;
using Finance.Domain.Enums;

namespace Finance.Application.Legacy.Dtos.Identities;

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
