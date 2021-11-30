namespace Fanex.NG2.ServiceBroker.Logging.LogEngines
{
    using Fanex.Logging;
    using Fanex.Logging.RabbitMQ;
    using Fanex.NG2.ServiceBroker.Configurations;

    public class RabbitLogEngine : LogEngineFactory
    {
        public ILogEngine Create()
            => new RabbitMQLogging(NG2ServiceBrokerConfig.RabbitMQConnectionStrings);
    }
}
