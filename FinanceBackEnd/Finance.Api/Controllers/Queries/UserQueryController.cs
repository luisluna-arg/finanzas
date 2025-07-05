using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.Users;
using Finance.Application.Queries.Users;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/users")]
public class UserQueryController(IMapper mapper, IMediator mediator)
    : BasicQueryController<User?, Guid, UserDto, GetAllUsersQuery, GetUserQuery>(mapper, mediator);
