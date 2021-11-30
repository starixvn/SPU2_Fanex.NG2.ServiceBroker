namespace Fanex.NG2.ServiceBroker.Logging
{
    using Fanex.Logging;

    public class LogManagerAdapter : LogManager
    {
        public new virtual LogManager SetDefaultLogCategory(string defaultCategory)
            => LogManager.SetDefaultLogCategory(defaultCategory);
    }
}
