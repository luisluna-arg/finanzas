using Finance.Application.Dtos.Base;
using Finance.Application.Dtos.Identities;

namespace Finance.Application.Dtos.Users;

public record UserDto : Dto<Guid>
{
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<IdentityDto> Identities { get; set; } = [];
    public ICollection<RoleDto> Roles { get; set; } = [];
    
    public UserDto()
    {
    }
}
