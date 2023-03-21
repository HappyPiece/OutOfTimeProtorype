using System.Security.Claims;
using LanguageExt.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.Dto;
using OutOfTimePrototype.Services.Authentication;
using OutOfTimePrototype.Services.General.Interfaces;
using OutOfTimePrototype.Utilities;
using static OutOfTimePrototype.Utilities.UserUtilities.UserOperationResult;

namespace OutOfTimePrototype.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly OutOfTimeDbContext _outOfTimeDbContext;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public AuthController(OutOfTimeDbContext outOfTimeDbContext, ITokenService tokenService,
            IUserService userService)
        {
            _tokenService = tokenService;
            _outOfTimeDbContext = outOfTimeDbContext;
            _userService = userService;
        }

        [HttpPost, Route("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!userDto.Validate().IsValid)
            {
                return BadRequest(new ValidationProblemDetails(userDto.Validate()));
            }

            var result = await _userService.TryRegisterUser(userDto);
            if (result.Status != OperationStatus.UserRegistered)
            {
                return StatusCode(Convert.ToInt32(result.HttpStatusCode), result.Message);
            }

            return await Login(new LoginDto
            {
                Password = userDto.Password,
                Email = userDto.Email
            });
        }

        [HttpPost, Route("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _outOfTimeDbContext.Users.FirstOrDefaultAsync(x => x.Email == loginDto.Email);

            if (user is null)
            {
                return BadRequest("Invalid credentials");
            }

            var hashedPassword = HashingHelper.ComputeSha256Hash(loginDto.Password);
            
            if (!(user.Password == hashedPassword || user.Password == loginDto.Password))
            {
                return BadRequest("Invalid credentials");
            }

            if (user.Password == loginDto.Password)
            {
                // If user has unhashed password then update the password in DB to hashed one
                var result = await _userService.EditUserPassword(user.Id, HashingHelper.ComputeSha256Hash(user.Password));
                if (result.Status != OperationStatus.UserEdited)
                {
                    throw new Exception("User password update failed");
                }
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            foreach (var role in user.VerifiedRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _outOfTimeDbContext.SaveChangesAsync();

            return Ok(new AuthenticateSuccessDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            });
        }

        [HttpPost, Route("refresh")]
        public async Task<IActionResult> Refresh(RefreshDTO refreshDTO)
        {
            var modelState = ModelState;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var principal = _tokenService.GetPrincipalFromToken(refreshDTO.AccessToken ??
                                                                throw new ArgumentNullException(
                                                                    "Access token not supplied"));

            var user = await _outOfTimeDbContext.Users.FirstOrDefaultAsync(x =>
                x.Id.ToString() == principal.Identity.Name);

            if (user is null || user.RefreshToken != refreshDTO.RefreshToken ||
                user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return BadRequest();
            }

            var accessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _outOfTimeDbContext.SaveChangesAsync();

            return Ok(new AuthenticateSuccessDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            });
        }

        [HttpPost, Authorize, Route("logout")]
        public async Task<IActionResult> Revoke()
        {
            var user = await _outOfTimeDbContext.Users.FirstOrDefaultAsync(x => x.Id.ToString() == User.Identity.Name);
            if (user is null)
            {
                return BadRequest();
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _outOfTimeDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}