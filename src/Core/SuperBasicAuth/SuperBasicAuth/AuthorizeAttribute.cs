
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Devmonster.Core.SuperBasicAuth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymous>().Any();
        if (allowAnonymous) return;

        var user = (User)context.HttpContext.Items["X-SuperBasicAuthUser"];
        if (user is null)
        {
            context.Result = new JsonResult(new { message = "Unauthorized", StatusCode = StatusCodes.Status401Unauthorized });
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }

    }
}
