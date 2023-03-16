﻿using Azure;
using Azure.Data.Tables;

namespace Devmonster.LoggerFluent.Models;

public class LogEntryEntity : ITableEntity
{
    public DateTime? DateStamp { get; set; } = DateTime.Now;

    public string LogLevel { get; set; }

    public string Category { get; set; }
    public string CorrelationId { get; set; }
    public string RelevantId { get; set; }

    public string Message { get; set; }
    public string PayloadSent { get; set; }
    public string ResponseReceived { get; set; }
    public string RelevantData { get; set; }
    public string Headers { get; set; }
    public string Url { get; set; }
    public string ClientID { get; set; }
    public string Exception { get; set; }

    public string FunctionExecutionID { get; set; }


    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

}
