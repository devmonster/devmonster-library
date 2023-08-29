namespace Devmonster.Core.LoggerFluent.ReferenceFunction;

public class LogWriterOptions
{
    public string StorageConnectionString { get; set; }
    public PartitionKeyOption PartitionKey { get; set; } = PartitionKeyOption.Category;
}

public enum PartitionKeyOption
{
    Guid,
    Category,
    Custom
}