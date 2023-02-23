using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfTimePrototype.Authorization;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.Services.General.Interfaces;
using OutOfTimePrototype.Utilities;
using static OutOfTimePrototype.Utilities.UserUtilities;

namespace OutOfTimePrototype.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly OutOfTimeDbContext _outOfTimeDbContext;
        private readonly IUserService _userService;

        public UserController(OutOfTimeDbContext outOfTimeDbContext, IUserService userService)
        {
            _outOfTimeDbContext = outOfTimeDbContext;
            _userService = userService;
        }

        [HttpGet, Authorize, Route("self")]
        public async Task<IActionResult> GetSelf()
        {
            if (User.Identity is null)
                throw new ArgumentNullException("User.Identity", "Expected not null User.Identity");
            if (!Guid.TryParse(User.Identity.Name, out var id))
                throw new ArgumentException("Expected User.Identity.Name to be valid string representation of Guid");
            return await GetUser(id);
        }

        [HttpGet, MinRoleAuthorize(Role.Admin), Route("{id:guid}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
        {
            var result = await _userService.TryGetUser(id);
            return result.Status != UserOperationResult.OperationStatus.Success
                ? StatusCode(Convert.ToInt32(result.HttpStatusCode), result.Message)
                : Ok(result.User);
        }

        [HttpGet, MinRoleAuthorize(Role.Admin), Route("unverified")]
        public async Task<IActionResult> GetUnverified()
        {
            throw new NotImplementedException();
        }

        [HttpPut, Route("{id:guid}/verify")]
        [MinRoleAuthorize(Role.Admin)]
        public async Task<IActionResult> VerifyRole([FromRoute] Guid id, [FromQuery] Role role)
        {
            var operationResult = await _userService.TryGetUser(id);
            if (operationResult.Status is not UserOperationResult.OperationStatus.Success)
            {
                return StatusCode(Convert.ToInt32(operationResult.HttpStatusCode), operationResult.Message);
            }

            var roles = User.Claims.Where(x => x.Type == ClaimTypes.Role).ToList();
            if (roles.Any(x => Enum.TryParse(x.Value, out Role userRole) && userRole.CanAssign(role)))
            {
                operationResult.User?.VerifiedRoles.Add(role);
                operationResult.User?.ClaimedRoles.Remove(role);
                await _outOfTimeDbContext.SaveChangesAsync();
                return Ok();
            }

            return StatusCode(403);
        }
    }
}