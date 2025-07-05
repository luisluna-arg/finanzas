using Finance.Domain.Enums;

namespace Finance.Application.Dtos.Identities;

public record IdentityDto() : Dto<Guid>
{
    public IdentityProviderEnum Provider { get; set; }
    public string SourceId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
