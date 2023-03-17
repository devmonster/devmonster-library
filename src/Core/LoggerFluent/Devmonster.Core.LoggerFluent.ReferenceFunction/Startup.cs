using Devmonster.AzureStorage.DataTable;
using Devmonster.Core.LoggerFluent.ReferenceFunction;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Devmonster.Core.LoggerFluent.ReferenceFunction;

internal class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        builder.Services.Configure<LogWriterOptions>(option => {
            option.StorageConnectionString = config["StorageConnectionString"];
        });

        builder.Services.AddScoped<IDataTableRepository, DataTableRepository>();

    }
}
