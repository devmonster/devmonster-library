namespace Devmonster.Core.LoggerFluent.Infrastructure;

public class LoggerFluentOptions
{
    public string ConnectionString { get; set; }

    /// <summary>
    /// Queue name to write the log to. Queue names may only be in lower-case and hypens <c>(-)</c>
    /// </summary>
    public string QueueName { get; set; }
}
