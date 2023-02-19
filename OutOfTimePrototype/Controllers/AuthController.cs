using LanguageExt.Pipes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.Dto;
using OutOfTimePrototype.Services.Authentication;
using OutOfTimePrototype.Services.Interfaces;
using System.Security.Claims;

namespace OutOfTimePrototype.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly OutOfTimeDbContext _outOfTimeDbContext;
        private readonly ITokenService _tokenService;

        public AuthController(OutOfTimeDbContext outOfTimeDbContext, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _outOfTimeDbContext = outOfTimeDbContext;
        }

        [HttpPost, Route("students/register")]
        public async Task<IActionResult> Register(StudentUserDto studentUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _outOfTimeDbContext.Users.AnyAsync(x => (x.Email == studentUserDto.Email)))
            {
                return BadRequest("User with such Email already exists");
            }

            var user = new StudentUser
            {
                Id = Guid.NewGuid(),
                Email = studentUserDto.Email,
                FirstName = studentUserDto.FirstName,
                LastName = studentUserDto.LastName,
                MiddleName = studentUserDto.MiddleName,
                Password = studentUserDto.Password,
                GradeBookNumber = studentUserDto.GradeBookNumber
            };

            user.ClaimedRoles.Add(Roles.Student);

            await _outOfTimeDbContext.Users.AddAsync(user);

            if (studentUserDto.Cluster is not null)
            {
                // TODO: assign cluster
            }

            await _outOfTimeDbContext.SaveChangesAsync();

            return await Login(new LoginDto
            {
                Password = studentUserDto.Password,
                Email = studentUserDto.Email
            });
        }

        [HttpPost, Route("educators/register")]
        public async Task<IActionResult> Register(EducatorUserDto educatorUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _outOfTimeDbContext.Users.AnyAsync(x => (x.Email == educatorUserDto.Email)))
            {
                return BadRequest("User with such Email already exists");
            }

            var user = new EducatorUser
            {
                Id = Guid.NewGuid(),
                Email = educatorUserDto.Email,
                FirstName = educatorUserDto.FirstName,
                LastName = educatorUserDto.LastName,
                MiddleName = educatorUserDto?.MiddleName,
                Password = educatorUserDto.Password
            };

            user.ClaimedRoles.Add(Roles.Educator);

            await _outOfTimeDbContext.Users.AddAsync(user);

            await _outOfTimeDbContext.SaveChangesAsync();

            return await Login(new LoginDto
            {
                Password = educatorUserDto.Password,
                Email = educatorUserDto.Email
            });
        }

        [HttpPost, Route("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _outOfTimeDbContext.Users.FirstOrDefaultAsync(x => (x.Email == loginDto.Email) && (x.Password == loginDto.Password));
            if (user is null)
            {
                return BadRequest("Invalid credentials");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var principal = _tokenService.GetPrincipalFromToken(refreshDTO.AccessToken ?? throw new ArgumentNullException("Access token not supplied"));

            var user = await _outOfTimeDbContext.Users.FirstOrDefaultAsync(x => x.Id.ToString() == principal.Identity.Name);

            if (user is null || user.RefreshToken != refreshDTO.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
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