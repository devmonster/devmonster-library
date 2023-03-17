using Microsoft.Extensions.DependencyInjection;

namespace Devmonster.Core.LoggerFluent.Infrastructure;

public static class ServiceHelper
{
    /// <summary>
    /// Configure LoggerFluent
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureLoggerFluent(this IServiceCollection services, Action<LoggerFluentOptions> loggerOptions)
    {      
        
        services.Configure(loggerOptions);
    }
}
