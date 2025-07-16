using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.Users;
using Finance.Application.Mapping;
using Finance.Application.Queries.Users;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/users")]
public class UserQueryController(IMappingService mapper, IDispatcher dispatcher)
    : BasicQueryController<User, Guid, UserDto, GetAllUsersQuery, GetUserQuery>(mapper, dispatcher);
