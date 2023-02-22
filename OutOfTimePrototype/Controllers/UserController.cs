using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.Services.General.Interfaces;
using OutOfTimePrototype.Services.Interfaces;
using OutOfTimePrototype.Utilities;
using System.Security.Claims;
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
            if (User.Identity is null) throw new ArgumentNullException("Expected not null User.Identity");
            if (!Guid.TryParse(User.Identity.Name, out Guid id)) throw new ArgumentException("Expected User.Identity.Name to be valid string representation of Guid");
            return await GetUser(id);
        }

        [HttpGet, Authorize(Roles = $"Root,Admin"), Route("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
        {
            var result = await _userService.TryGetUser(id);
            if (result.Status != UserOperationResult.OperationStatus.Success)
            {
                return StatusCode(Convert.ToInt32(result.HttpStatusCode), result.Message);
            }
            return Ok(result.User);
        }

        [HttpGet, Authorize(Roles = $"Root,Admin"), Route("unverified")]
        public async Task<IActionResult> GetUnverified()
        {
            throw new NotImplementedException();
        }

        [HttpPut, Route("{id}/verify")]
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
                operationResult.User.VerifiedRoles.Add(role);
                operationResult.User.ClaimedRoles.Remove(role);
                _outOfTimeDbContext.SaveChanges();
                return Ok();
            }
            else
            {
                return StatusCode(403);
            }
           
        }
    }
}
