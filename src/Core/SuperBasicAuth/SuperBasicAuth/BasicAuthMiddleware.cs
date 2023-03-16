using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text;

namespace Devmonster.Core.SuperBasicAuth;

public sealed class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;

    public BasicAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IBasicAuthService authService)
    {
        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            var username = credentials[0];
            var password = credentials[1];


            if (await authService.Authenticate(username, password))
            {
                context.Items["X-SuperBasicAuthUser"] = new User() { Username = username };
            }
        }
        catch
        {

        }

        await _next(context);
    }
}

public static class SuperBasicAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseSuperBasicAuthentication(this IApplicationBuilder builder)
    {        
        return builder.UseMiddleware<BasicAuthMiddleware>();
    }

    public static void AddSuperBasicAuthentication(this IServiceCollection services, Action<SuperBasicAuthConfig> options)
    {
        services.Configure(options);
        services.AddSingleton<IBasicAuthService, BasicAuthService>();
    }
}
