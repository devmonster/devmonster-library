namespace Devmonster.Core.LoggerFluent.Models;

public class LogEntry
{
    /// <summary>
    /// If DateStamp is not specified, DateTime will be set to the LogEntry instance creation
    /// </summary>
    public DateTime DateStamp { get; set; } = DateTime.Now;


    /// <summary>
    /// Log Level.
    /// <para />
    /// Use <see cref="LogLevel"/> for values
    /// </summary>
    public string LogLevel { get; set; }

    public string Category { get; set; }
    public string CorrelationId { get; set; }
    public string RelevantId { get; set; }

    public string Message { get; set; }
    public object PayloadSent { get; set; }
    public string ResponseReceived { get; set; }
    public object RelevantData { get; set; }
    public object Headers { get; set; }
    public string Url { get; set; }
    public string ClientID { get; set; }
    public string FunctionExecutionID { get; set; }
    public string Exception { get; set; }

}

public class LogLevel
{
    public static string Trace { get => Levels[0]; }
    public static string Debug { get => Levels[1]; }
    public static string Info { get => Levels[2]; }
    public static string Warning { get => Levels[3]; }
    public static string Error { get => Levels[4]; }
    public static string Fatal { get => Levels[5]; }

    public readonly static string[] Levels = { "TRACE", "DEBUG", "INFO", "WARNING", "ERROR", "FATAL" };
}
