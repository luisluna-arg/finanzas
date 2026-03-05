using Finance.Application.Legacy.Dtos.Users;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Auth;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class UserMapper : BaseMapper<User, UserDto>, IUserMapper
{
    public UserMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IUserMapper : IMapper<User, UserDto>;
