using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.Utilities;

namespace OutOfTimePrototype.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class MinRoleAuthorizeAttribute : TypeFilterAttribute
{
    public MinRoleAuthorizeAttribute(Role minRole) : base(typeof(AuthorizeActionFilter))
    {
        Arguments = new object[] { minRole };
    }
}

public class AuthorizeActionFilter : IAuthorizationFilter
{
    private readonly Role _minRole;

    public AuthorizeActionFilter(Role minRole)
    {
        _minRole = minRole;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var roles = context.HttpContext.User.Claims.Where(claim => claim.Type == ClaimTypes.Role).Select(claim => claim.Value).ToList();
        
        foreach (var role in roles)
        {
            var isSucceed = Enum.TryParse(role, out Role enumRole);

            if (!isSucceed)
            {
                context.Result = new BadRequestObjectResult("The user have invalid roles specified");
                return;
            }

            if (_minRole.IsHigherOrEqualPermissions(enumRole)) return;  
        }
        
        context.Result = new ForbidResult();
    }
}