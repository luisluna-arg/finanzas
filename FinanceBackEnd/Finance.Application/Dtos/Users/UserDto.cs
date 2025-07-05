using Finance.Application.Dtos.Identities;

namespace Finance.Application.Dtos.Users;

public record UserDto() : Dto<Guid>
{
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<IdentityDto> Identities { get; set; } = [];
    public ICollection<RoleDto> Roles { get; set; } = [];
}
