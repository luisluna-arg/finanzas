using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Queries.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Finance.Api.Controllers
{
    /// <summary>
    /// Provides information about the currently authenticated user session.
    /// </summary>
    [ApiController]
    [Route("api/session")]
    [ApiExplorerSettings(GroupName = "Session")]
    [SwaggerTag("Session")]
    public class SessionController(IDispatcher<FinanceDispatchContext> dispatcher) : ControllerBase
    {
        private IDispatcher<FinanceDispatchContext> Dispatcher { get => dispatcher; }

        /// <summary>
        /// Gets the current logged-in user's claims.
        /// </summary>
        /// <returns>Basic user information from claims.</returns>
        [HttpGet("me")]
        [Authorize(Policy = "AuthenticatedPolicy")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var sourceId = HttpContext.User.Identity?.Name ?? string.Empty;

            var userResult = await Dispatcher.DispatchQueryAsync(new GetUserBySourceIdsQuery([sourceId]));

            if (!userResult.IsSuccess || userResult.Data == null)
            {
                return NotFound(new { Error = "User not found" });
            }

            var claims = HttpContext.User.Claims
                .GroupBy(c => c.Type)
                .ToDictionary(g => g.Key, g => g.Select(c => c.Value).ToList());

            return Ok(new
            {
                UserId = userResult.Data.Id,
                FullName = $"{userResult.Data!.FirstName} {userResult.Data.LastName}",
                Username = userResult.Data.Username,
                SourceId = sourceId,
                Roles = userResult.Data.Roles.Select(r => r.Name).ToList(),
                Claims = claims,
                userResult.Data.CreatedAt,
                userResult.Data.UpdatedAt
            });
        }
    }
}
