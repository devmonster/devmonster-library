using Devmonster.AzureStorage.DataTable;
using Devmonster.AzureStorage.Queue;
using Devmonster.Core.LoggerFluent.Infrastructure;
using Devmonster.Core.LoggerFluent.LoggerFluent;
using Devmonster.Core.LoggerFluent.Test;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Devmonster.Core.LoggerFluent.Test
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddTransient<ILoggerFluent, LoggerFluent.LoggerFluent>();
            builder.Services.AddTransient<IQueueRepository, QueueRepository>();

            builder.Services.ConfigureLoggerFluent(o =>
            {
                o.QueueName = "test";
                o.ConnectionString = config["ConnectionString"];
            });


            builder.Services.AddScoped<IDataTableRepository, DataTableRepository>();

        }
    }

    public class LogWriterOptions
    {
        public string StorageConnectionString { get; set; }
    }
}
