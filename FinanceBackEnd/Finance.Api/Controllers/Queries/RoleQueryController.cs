using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.Users;
using Finance.Application.Mapping;
using Finance.Application.Queries.Roles;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/roles")]
public class RoleQueryController(IMappingService mapper, IDispatcher dispatcher)
    : BasicQueryController<Role, RoleEnum, RoleDto, GetAllRolesQuery, GetRoleQuery>(mapper, dispatcher);
