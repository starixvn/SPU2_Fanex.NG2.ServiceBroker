namespace Fanex.NG2.ServiceBroker.Logging
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.NG2.ServiceBroker.Configurations;
    using Fanex.NG2.ServiceBroker.Logging.LogEngines;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class LoggingConfig
    {
        [ThreadStatic]
        private static LoggingConfiguration loggingConfiguration;

        public static LoggingConfiguration LoggingConfiguration
        {
            get => loggingConfiguration ?? new LoggingConfiguration();
            set => loggingConfiguration = value;
        }

        public static void Init(Func<Exception, IDictionary<string, object>> getDbExceptionInfo = null)
        {
            var libs = NG2ServiceBrokerConfig.LogLibs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var logEngines = libs.Select(CreateLogLib).Where(engine => engine != null);
            var logDefaultCategory = NG2ServiceBrokerConfig.LoggingDefaultCategory;

            LogManager.SetDefaultLogCategory(logDefaultCategory).Use(logEngines);
        }

        public static ILogEngine CreateLogLib(string libName)
        {
            var type = Type.GetType($"Fanex.NG2.ServiceBroker.Logging.LogEngines.{libName}LogEngine");
            if (type == null)
            {
                throw new ArgumentException("Cannot create LogEngine. Please check LogEngine's name config.");
            }

            var engine = Activator.CreateInstance(type) as LogEngineFactory;

            return engine?.Create();
        }

        public static List<string> GetRegexToHideSensitiveInfo(IEnumerable<string> sensitiveInfo)
        {
            var result = new List<string>();
            sensitiveInfo?.ToList().ForEach(info =>
            {
                result.Add($"(?:{info}=)(?<paramValue>[^&|^\\s]+)");
                result.Add($"(\"{info}\":)(?<paramValue>[^,|^\r]+)");
            });

            return result;
        }
    }
}
