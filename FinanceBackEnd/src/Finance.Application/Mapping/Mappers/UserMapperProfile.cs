using Finance.Application.Dtos.Users;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.Auth;

namespace Finance.Application.Mapping.Mappers;

public class UserMapper : BaseMapper<User, UserDto>, IUserMapper
{
    public UserMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IUserMapper : IMapper<User, UserDto>;
