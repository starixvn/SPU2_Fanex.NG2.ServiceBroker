namespace Fanex.NG2.ServiceBroker.Logging.LogEngines
{
    using Fanex.Logging;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NG2.ServiceBroker.Configurations;

    public class SentryLogEngine : LogEngineFactory
    {
        public ILogEngine Create()
        {
            var sentryOptions = new SentryEngineOptions
            {
                Dsn = new SharpRaven.Dsn(NG2ServiceBrokerConfig.SentryBackendUrl)
            };

            return new SentryLogging(sentryOptions);
        }
    }
}
