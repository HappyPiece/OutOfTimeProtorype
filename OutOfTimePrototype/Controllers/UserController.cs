using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfTimePrototype.Authorization;
using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.Dto;
using OutOfTimePrototype.Exceptions;
using OutOfTimePrototype.Services.General.Interfaces;
using OutOfTimePrototype.Utilities;

namespace OutOfTimePrototype.Controllers;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    private Guid? GetUserIdFromToken()
    {
        var id = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
        return id is null ? null : Guid.Parse(id);
    }

    private List<Role>? ExtractRoles()
    {
        try
        {
            return User.Claims.Where(x => x.Type == ClaimTypes.Role)
                .Select(claim => Enum.Parse<Role>(claim.Value)).ToList();
        }
        catch (ArgumentException e)
        {
            return null;
        }
    }

    [HttpGet]
    [Authorize]
    [Route("self")]
    public async Task<IActionResult> GetSelf()
    {
        if (User.Identity is null)
            throw new ArgumentNullException("User.Identity", "Expected not null User.Identity");

        if (!Guid.TryParse(User.Identity.Name, out var id))
            throw new ArgumentException("Expected User.Identity.Name to be valid string representation of Guid");

        return await GetUser(id);
    }

    [HttpGet]
    [MinRoleAuthorize(Role.Admin)]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetUser([FromRoute] Guid id)
    {
        var user = await _userService.GetUser(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPut]
    [Route("{id:guid}/edit")]
    public async Task<IActionResult> Edit([FromRoute] Guid id, [FromBody] UserDto userDto)
    {
        var userId = GetUserIdFromToken();

        if (userId is null)
            return BadRequest("User id is not specified in token");

        // If user tries to edit someone else...
        if (userId != id)
        {
            var roles = ExtractRoles();

            if (roles is null)
                return BadRequest("User roles is not specified in token");

            // ...check if he can do it
            var canAccess = roles.Any(r => r.IsHigherOrEqualPermissions(Role.Admin));

            if (!canAccess)
                return Forbid();
        }

        if (ModelState.IsValid) return BadRequest(ModelState);

        var userDtoModelState = userDto.Validate();
        if (!userDtoModelState.IsValid) return BadRequest(userDtoModelState);

        var result = await _userService.EditUser(id, userDto);
        return UserUtilities.UserOperationResult.ToIActionResult(result);
    }

    [HttpDelete]
    [MinRoleAuthorize(Role.Admin)]
    [Route("{id:guid}/delete")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _userService.DeleteUser(id);
        return UserUtilities.UserOperationResult.ToIActionResult(result);
    }

    [HttpGet]
    [MinRoleAuthorize(Role.Admin)]
    [Route("{id:guid}/unverified")]
    public async Task<IActionResult> GetUnverifiedRoles(Guid id)
    {
        var result = await _userService.GetUnverifiedRoles(id);
        return result.Match<IActionResult>(
            Ok,
            e => e is RecordNotFoundException ? NotFound(e.Message) : StatusCode(500)
        );
    }

    [HttpPut]
    [MinRoleAuthorize(Role.Admin)]
    [Route("{id:guid}/verify")]
    public async Task<IActionResult> VerifyRole([FromRoute] Guid id, [FromQuery] Role role)
    {
        var examinerRoles = ExtractRoles();

        if (examinerRoles is null)
            return BadRequest();

        var result = await _userService.VerifyUserRole(examinerRoles, id, role);
        return result.Match<IActionResult>(
            NoContent,
            e =>
            {
                return e switch
                {
                    AccessNotAllowedException => Forbid(e.Message),
                    RecordNotFoundException => NotFound(e.Message),
                    _ => StatusCode(500)
                };
            }
        );
    }
}