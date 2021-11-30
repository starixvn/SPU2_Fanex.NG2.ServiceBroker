using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NG2.ServiceBroker.Configurations
{
    public class NG2ServiceBrokerConfig
    {
        public static int MaxRetryCount => Convert.ToInt32(ConfigurationManager.AppSettings["MaxRetryCount"]);

        public static string LoggingDefaultCategory => ConfigurationManager.AppSettings["Fanex.Logging:DefaultCategory"];

        public static string LogLibs => ConfigurationManager.AppSettings["LogLibs"];

        public static string RabbitMQConnectionStrings => ConfigurationManager.AppSettings["Fanex.Logging:RabbitMQConnectionStrings"];

        public static string SentryBackendUrl => ConfigurationManager.AppSettings["Fanex.Logging:SentryBackendUrl"];

        public static string SentryUrl => ConfigurationManager.AppSettings["Fanex.Logging:SentryUrl"];
    }
}
