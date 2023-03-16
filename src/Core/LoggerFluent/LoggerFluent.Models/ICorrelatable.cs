namespace Devmonster.LoggerFluent.Models;

public interface ICorrelatable
{
    string CorrelationId { get; set; }
}
