using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Base;

[ApiController]
[Authorize(Policy = "AdminPolicy")]
[Produces("application/json")]
public abstract class SecuredApiController : ControllerBase
{
    // Base functionality for secured controllers
}
