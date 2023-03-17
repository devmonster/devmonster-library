using Devmonster.Core.LoggerFluent.LoggerFluent;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Devmonster.Core.LoggerFluent.Test
{
    public class Function1
    {
        ILoggerFluent _loggerClient;

        public Function1(ILoggerFluent loggerClient)
        {
            _loggerClient = loggerClient;
        }

        [FunctionName("Function1")]
        public void Run([QueueTrigger("myqueue-items", Connection = "")]string myQueueItem, ILogger log)
        {
            var client = _loggerClient.CreateNew();

            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }


    }
}
