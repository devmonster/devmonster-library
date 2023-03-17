using System;
using Azure.Storage.Queues.Models;
using System.Reflection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Devmonster.Core.LoggerFluent.Models;
using Devmonster.AzureStorage.DataTable;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Devmonster.Core.LoggerFluent.ReferenceFunction
{
    public class Function1
    {        
        private string TableName = "EventLog";
        private readonly int maxStringLength = 5000;
        private readonly IDataTableRepository _storage;
        private readonly IOptions<LogWriterOptions> _options;

        public Function1(IDataTableRepository storage,
            IOptions<LogWriterOptions> options)
        {
            _storage = storage;
            _options = options;

            _storage.SetConnectionString(_options.Value.StorageConnectionString);

        }
        [FunctionName("Function1")]
        public async Task Run([QueueTrigger("EventLog", Connection = "")]string queueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {queueItem}");

            var logEntry = JsonConvert.DeserializeObject<LogEntry>(queueItem);

            LogEntryEntity entity = new()
            {
                DateStamp = logEntry.DateStamp.ToUniversalTime(),
                Category = logEntry.Category,
                CorrelationId = logEntry.CorrelationId,
                PayloadSent = logEntry.PayloadSent?.TrimToLength(maxStringLength),
                RelevantData = JsonConvert.SerializeObject(logEntry.RelevantData).TrimToLength(maxStringLength),
                ResponseReceived = logEntry.ResponseReceived?.TrimToLength(maxStringLength),
                LogLevel = logEntry.LogLevel,
                Message = logEntry.Message?.TrimToLength(maxStringLength),
                RelevantId = logEntry.RelevantId,
                Url = logEntry.Url,
                ClientID = logEntry.ClientID,
                Headers = JsonConvert.SerializeObject(logEntry.Headers),

                PartitionKey = logEntry.Category,
                RowKey = Guid.NewGuid().ToString()
            };

            try
            {


                await _storage.AddOrUpdateAsync(TableName, entity);
            }
            catch (Exception ex)
            {
                string errorCorrelationId = logEntry.CorrelationId == string.Empty ? Guid.NewGuid().ToString() : logEntry.CorrelationId;
                LogEntryEntity errorEntry = new()
                {
                    DateStamp = DateTime.Now,
                    Category = "Logger Error",
                    CorrelationId = errorCorrelationId,
                    PayloadSent = JsonConvert.SerializeObject(logEntry),
                    LogLevel = Models.LogLevel.Error,
                    Message = ex.Message,
                    RelevantData = JsonConvert.SerializeObject(ex),

                    PartitionKey = "Logger Error",
                    RowKey = Guid.NewGuid().ToString()
                };

                await _storage.AddOrUpdateAsync(TableName, errorEntry);

            }
        }
    }
}
