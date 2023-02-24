using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfTimePrototype.Authorization;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.Dto;
using OutOfTimePrototype.Exceptions;
using OutOfTimePrototype.Services.General.Interfaces;
using OutOfTimePrototype.Utilities;

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
            var user = await _userService.GetUser(id);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpPost, MinRoleAuthorize(Role.Admin), Route("create")]
        public async Task<IActionResult> Create()
        {
            throw new NotImplementedException();
        }
        
        [HttpPut, MinRoleAuthorize(Role.Admin), Route("{id:guid}/edit")]
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromBody] UserDto userDto)
        {
            if (ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDtoModelState = userDto.Validate();

            if (!userDtoModelState.IsValid)
            {
                return BadRequest(userDtoModelState);
            }

            var result = await _userService.EditUser(id, userDto);
            return UserUtilities.UserOperationResult.ToIActionResult(result);
        }

        [HttpDelete, MinRoleAuthorize(Role.Admin), Route("{id:guid}/delete")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _userService.DeleteUser(id);
            return UserUtilities.UserOperationResult.ToIActionResult(result);
        }

        [HttpGet, MinRoleAuthorize(Role.Admin), Route("{id:guid}/unverified")]
        public async Task<IActionResult> GetUnverifiedRoles(Guid id)
        {
            var result = await _userService.GetUnverifiedRoles(id);
            return result.Match<IActionResult>(
                Ok,
                e => e is RecordNotFoundException ? NotFound(e.Message) : StatusCode(500)
            );
        }

        // TODO: move to service
        [HttpPut, MinRoleAuthorize(Role.Admin), Route("{id:guid}/verify")]
        public async Task<IActionResult> VerifyRole([FromRoute] Guid id, [FromQuery] Role role)
        {
            var userToApprove = await _userService.GetUser(id);
            if (userToApprove is null)
                return NotFound();

            var examinerRoleClaims = User.Claims.Where(x => x.Type == ClaimTypes.Role).ToList();
            if (!examinerRoleClaims.Any(x =>
                    Enum.TryParse(x.Value, out Role examinerRole) && examinerRole.CanAssign(role)))
                return Forbid();

            userToApprove.VerifiedRoles.Add(role);
            userToApprove.ClaimedRoles.Remove(role);
            _outOfTimeDbContext.Users.Update(userToApprove);
            await _outOfTimeDbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}