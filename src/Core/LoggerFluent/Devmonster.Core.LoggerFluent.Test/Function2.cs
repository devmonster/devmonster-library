using Devmonster.Core.LoggerFluent;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;



namespace Devmonster.Core.LoggerFluent.Test
{
    public class Function2        
    {
        readonly ILoggerFluent _loggerClient;

        public Function2(ILoggerFluent loggerClient)
        {
            _loggerClient = loggerClient;
        }

        [FunctionName("Function2")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                var logger = _loggerClient.CreateNew();

                logger.BeginEntry("CorrelationId", "Category");
                logger.Trace("This is a trace message").Log();


                string name = req.Query["name"];

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                name = name ?? data?.name;

                string responseMessage = string.IsNullOrEmpty(name)
                    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                    : $"Hello, {name}. This HTTP triggered function executed successfully.";

                logger.Info("Sending Response")
                    .Message(responseMessage)
                    .Payload(data)
                    .RelevantId("3393")
                    .AdditionalData(new { name = "data1", food = "cookies" })
                    .Response(new { response = "ok", message = "the response is successfulley"})
                    .Log();

                logger.Info("This is a test").AdditionalData("This is a string").Log();

                var wikipedia = await "https://en.wikipedia.org".GetStringAsync();
                logger.Trace("From Wikipedia").Response(wikipedia, maxCharacters: 20).Log();

                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                return new OkResult();
            }
        }
    }
}
