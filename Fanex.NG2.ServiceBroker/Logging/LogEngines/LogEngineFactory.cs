namespace Fanex.NG2.ServiceBroker.Logging.LogEngines
{
    using Fanex.Logging;

    public interface LogEngineFactory
    {
        ILogEngine Create();
    }
}
