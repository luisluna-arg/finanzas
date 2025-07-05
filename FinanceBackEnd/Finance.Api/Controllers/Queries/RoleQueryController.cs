using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.Users;
using Finance.Application.Queries.Roles;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/roles")]
public class RoleQueryController(IMapper mapper, IMediator mediator)
    : BasicQueryController<Role?, RoleEnum, RoleDto, GetAllRolesQuery, GetRoleQuery>(mapper, mediator);
