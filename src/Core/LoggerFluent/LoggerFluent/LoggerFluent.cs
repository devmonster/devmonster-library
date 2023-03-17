using Devmonster.AzureStorage.Queue;
using Devmonster.Core.Extensions.String;
using Devmonster.Core.LoggerFluent.Infrastructure;
using Devmonster.Core.LoggerFluent.Models;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Text;

namespace Devmonster.Core.LoggerFluent.LoggerFluent;

#region Interface
public interface ILoggerFluent
{
    /// <summary>
    /// Creates a new instance of the LoggerFluent
    /// </summary>
    /// <returns></returns>
    ILoggerFluent CreateNew();
    ILoggerFluent CreateNew(string correlationId, [CallerMemberName] string category = "");

    /// <summary>
    /// Starts a new Logging Transaction
    /// </summary>
    /// <param name="correlationId">The Correlation ID the logging transaction belongs to</param>
    /// <returns></returns>
    ILoggerFluent BeginEntry(string correlationId, [CallerMemberName] string category = "");


    ILoggerFluent ForCategory(string category);

    ILoggerFluent ForRelevantId(string id);

    ILoggerFluent ClientID(string clientId);

    ILoggerFluent FunctionExecutionID(string executionID);

    /// <summary>
    /// Logs a Trace level event
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    ILoggerData Trace(string message);

    /// <summary>
    /// Logs a Debug level event
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    ILoggerData Debug(string message);

    /// <summary>
    /// Logs a Info level event
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    ILoggerData Info(string message);

    /// <summary>
    /// Logs a Warning level event
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    ILoggerData Warn(string message);

    /// <summary>
    /// Logs a Error level event
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    ILoggerData Error(string message);


    /// <summary>
    /// Logs a Fatal level event
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    ILoggerData Fatal(string message);

    /// <summary>
    /// Clears all data in the current log instance
    /// </summary>
    void ClearAll();

    /// <summary>
    /// Clears transactional fields <para/>
    /// 
    /// <see cref="LogEntry.LogLevel"/> <para/>
    /// <see cref="LogEntry.Message"/> <para/>        
    /// <see cref="LogEntry.PayloadSent"/> <para/>
    /// <see cref="LogEntry.ResponseReceived"/><para />
    /// <see cref="LogEntry.RelevantData"/>
    /// </summary>
    void ClearTransaction();

    /// <summary>
    /// Writes a full log entry using a specified <see cref="LogEntry"/>
    /// </summary>
    /// <param name="logEntry"></param>
    Task Write(LogEntry logEntry);

    /// <summary>
    /// Returns an instance of the <see cref="LogEntry"/> currently stored in the Fluent Logger
    /// </summary>
    /// <returns></returns>
    LogEntry GetLogEntry();
}

public interface ILoggerConfigure
{
    /// <summary>
    /// Sets the Relevant ID for the current log entry
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    ILoggerData RelevantId(string id);
}

public interface ILoggerData
{


    /// <summary>
    /// Sets the Category for the current log entry
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    ILoggerData Category(string category);

    /// <summary>
    /// Sets a custom Date Stamp for the log entry. 
    /// If this is not set, <c>DateTime.Now</c> is used.
    /// <para />
    /// <c>BeginWrite</c> clears the Date Stamp        
    /// </summary>
    /// <param name="stamp">Custom <see cref="DateTime"/> value to write to the log</param>
    /// <returns></returns>
    ILoggerData DateStamp(DateTime stamp);

    /// <summary>
    /// Sets the value of the Payload sent to an API that will be written on the log entry
    /// </summary>
    /// <param name="payloadSent"></param>
    /// <returns></returns>
    ILoggerData Payload(string payloadSent);

    /// <summary>
    /// Sets the value of the API response that will be written on the log entry
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    ILoggerData Response(string response);

    /// <summary>
    /// Sets a custom object that will be written on the log entry
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    ILoggerData AdditionalData(object data);

    ILoggerData Url(string url);

    ILoggerData Headers(object headers);

    /// <summary>
    /// Adds an Exception to the log, which will be automatically Unfurled
    /// </summary>    
    ILoggerData Exception(Exception ex);

    ILoggerData Message(string message);

    /// <summary>
    /// Commits the log entry to the Log Writer and ends the Log Transaction
    /// <para />
    /// This also clears the fields:<para/>
    /// <see cref="LogEntry.LogLevel"/> <para/>
    /// <see cref="LogEntry.Message"/> <para/>
    /// <see cref="LogEntry.Category"/> <para />
    /// <see cref="LogEntry.PayloadSent"/> <para/>
    /// <see cref="LogEntry.ResponseReceived"/><para />
    /// <see cref="LogEntry.RelevantData"/><para />
    /// <see cref="LogEntry.Exception"/>
    /// </summary>
    /// <returns></returns>
    void Log();
}

#endregion Interface


#region Implementation
public sealed class LoggerFluent : ILoggerFluent, ILoggerConfigure, ILoggerData
{
    private string _correlationId;
    private string _level;
    private string _relevantId;
    private DateTime? _stamp;
    private string _payload;
    private string _response;
    private string _data;
    private string _message;
    private string _category;
    private string _url;
    private object _headers;
    private string _executionID;
    private string _exception;

    private bool hasDateSet = false;
    private string _clientID;
    private readonly IQueueRepository _queue;
    private readonly IOptions<LoggerFluentOptions> _options;


    public LoggerFluent(
        IQueueRepository queue, IOptions<LoggerFluentOptions> options)
    {
        _queue = queue;
        _options = options;

        _queue.SetConnectionString(options.Value.ConnectionString);
    }

    /// <summary>
    /// Creates a new instance of the LoggerFluent
    /// </summary>
    /// <returns></returns>
    public ILoggerFluent CreateNew()
    {
        return new LoggerFluent(_queue, _options);
    }


    /// <summary>
    /// Creates a new instance of the LoggerFluent
    /// </summary>
    /// <returns></returns>
    public ILoggerFluent CreateNew(string correlationId, [CallerMemberName] string category = "")
    {
        var loggerInstance = new LoggerFluent(_queue, _options);
        return loggerInstance.BeginEntry(correlationId, category);
    }

    public ILoggerData AdditionalData(object data)
    {
        var jsonFormattingSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        string dataString = JsonConvert.SerializeObject(data, jsonFormattingSettings);

        _data = dataString;
        return this;
    }

    public ILoggerFluent BeginEntry(string correlationId, [CallerMemberName] string category = "")
    {
        _correlationId = correlationId;

        if (string.IsNullOrEmpty(_category))
        {
            _category = category;
        }

        return this;
    }

    public ILoggerFluent FunctionExecutionID(string executionID)
    {
        _executionID = executionID;

        return this;
    }

    //public ILoggerAction BeginWrite(string level)
    //{
    //    // Reset the values so it does not bleed into other logs

    //    // Set this to False so that we reset it 
    //    // if this is set to false, the logger uses the DateTime.Now
    //    hasDateSet = false;

    //    _payload = _relevantId = _response = _message = string.Empty;
    //    _data = null;


    //    // Set the level
    //    _level = level;
    //    return this;
    //}

    public ILoggerData DateStamp(DateTime stamp)
    {
        _stamp = stamp;

        hasDateSet = true;

        return this;
    }

    public async void Log()
    {

        if (string.IsNullOrEmpty(_category))
        {
            throw new ArgumentNullException(nameof(Category), "Value cannot be null");
        }

        LogEntry entry = new()
        {
            CorrelationId = _correlationId,
            LogLevel = _level,
            RelevantId = _relevantId,
            PayloadSent = _payload,
            ResponseReceived = _response?.TrimToLength(5000),
            RelevantData = _data?.TrimToLength(5000),
            Message = _message,
            Category = _category,
            ClientID = _clientID,
            Exception = _exception,
            FunctionExecutionID = _executionID
        };

        if (hasDateSet)
            entry.DateStamp = _stamp.Value;
        else
            entry.DateStamp = DateTime.Now;

        await _queue.AddMessageQueueAsync<LogEntry>(_options.Value.QueueName, entry);

        ClearTransaction();
    }

    public ILoggerData Message(string message)
    {
        _message = message;
        return this;
    }

    public ILoggerData Payload(string payloadSent)
    {
        _payload = payloadSent;
        return this;
    }

    public ILoggerData RelevantId(string id)
    {
        _relevantId = id;
        return this;
    }

    public ILoggerData Response(string response)
    {
        _response = response;
        return this;
    }

    public ILoggerData Exception(Exception ex)
    {
        StringBuilder returnString = new StringBuilder();
        returnString.AppendLine($"Message: {ex.Message} ");
        returnString.AppendLine(ex.StackTrace);

        Exception inner = ex.InnerException;

        while (inner != null)
        {
            returnString.AppendLine($" Inner: {inner.Message}");
            inner = inner.InnerException;
        }

        _exception = returnString.ToString();

        return this;
    }

    public ILoggerData Category(string category)
    {
        _category = category;
        return this;
    }


    public void ClearAll()
    {
        _correlationId = string.Empty;
        _level = string.Empty;
        _relevantId = string.Empty;
        _stamp = DateTime.Now;
        _payload = string.Empty;
        _response = string.Empty;
        _data = null;
        _message = string.Empty;
        _category = string.Empty;
        _executionID = string.Empty;
        _exception = string.Empty;

        hasDateSet = false;
    }

    public async Task Write(LogEntry logEntry)
    {
        if (_category == string.Empty)
        {
            throw new ArgumentException("Category value is required");
        }

        await _queue.AddMessageQueueAsync(_options.Value.QueueName, JsonConvert.SerializeObject(logEntry));

    }

    public LogEntry GetLogEntry()
    {
        return new LogEntry()
        {
            CorrelationId = _correlationId,
            LogLevel = _level,
            RelevantId = _relevantId,
            PayloadSent = _payload,
            ResponseReceived = _response,
            RelevantData = _data,
            Headers = _headers,
            Url = _url,
            Message = _message,
            Category = _category,
            ClientID = _clientID,
            Exception = _exception,
            FunctionExecutionID = _executionID
        };
    }

    public void ClearTransaction()
    {
        // Clear the repeating part of the log so it can be reused
        _payload = _response = _message = _level = _exception = string.Empty;

        _data = null;

    }

    public ILoggerData Trace(string message)
    {
        _message = message;
        _level = LogLevel.Trace;

        return this;
    }

    public ILoggerData Debug(string message)
    {
        _message = message;
        _level = LogLevel.Debug;

        return this;
    }

    public ILoggerData Info(string message)
    {
        _message = message;
        _level = LogLevel.Info;

        return this;
    }

    public ILoggerData Warn(string message)
    {
        _message = message;
        _level = LogLevel.Warning;

        return this;
    }

    public ILoggerData Error(string message)
    {
        _message = message;
        _level = LogLevel.Error;

        return this;
    }

    public ILoggerData Fatal(string message)
    {
        _message = message;
        _level = LogLevel.Fatal;

        return this;
    }

    public ILoggerFluent ForCategory(string category)
    {
        _category = category;

        return this;
    }

    public ILoggerFluent ForRelevantId(string id)
    {
        _relevantId = id;

        return this;
    }

    public ILoggerData Url(string url)
    {
        _url = url;

        return this;
    }

    public ILoggerData Headers(object headers)
    {
        _headers = headers;
        return this;
    }

    public ILoggerFluent ClientID(string clientID)
    {
        _clientID = clientID;

        return this;
    }




}
#endregion Implementation
