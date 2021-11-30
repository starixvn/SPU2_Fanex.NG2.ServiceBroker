using EasyNetQ;
using Fanex.Logging.Extensions.Database;
using Fanex.NG2.ServiceBroker.Logging;
using ONEbook.NG2.API.Configuration;
using ONEbook.NG2.API.DTO.CreateAccount;
using System;
using System.Reflection;
using System.ServiceProcess;

namespace Fanex.NG2.ServiceBroker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            LoggingConfig.Init(DatabaseInfoGetters.GetDbExceptionInfo);
            ServiceBase[] servicesToRun = new ServiceBase[]
            {
                new NG2ServiceBroker()
            };

            if (Environment.UserInteractive)
            {
                Type type = typeof(ServiceBase);
                BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                MethodInfo method = type.GetMethod("OnStart", flags);
                foreach (ServiceBase service in servicesToRun)
                {
                    method.Invoke(service, new object[] { args });
                }

                Console.WriteLine("Client Server Service has been started.");
                Console.WriteLine("Press Exit to exit!");
                while (true)
                {
                    var command = Console.ReadLine();

                    if (command == "Publish")
                    {
                        PublishMessage();
                    }


                    if (command == "Exit")
                    {
                        break;
                    }
                }
                foreach (ServiceBase service in servicesToRun)
                {
                    service.Stop();
                }
            }
            else
            {
                ServiceBase.Run(servicesToRun);
            }
        }

        public static void PublishMessage()
        {
            var NG2Configuration = ONEbook.NG2.API.Configuration.NG2Configuration.Instance;
            var retrySyncRequest = new Request();
            retrySyncRequest.Auth.SiteId = 2000;
            retrySyncRequest.UserId = 8;
            retrySyncRequest.Call.CurrencyId = 1;
            retrySyncRequest.Call.AccountArray.Add(new AccountInfo { ObCustId = 123456 });
            retrySyncRequest.Call.Level = 4;
            retrySyncRequest.Call.UplineId = 123456;

            IBus bus = null;
            bus = RabbitHutch.CreateBus(
               string.Format("host={0};virtualHost={1};username={2};password={3}", NG2Configuration.RabbitHost, NG2Configuration.RabbitName, NG2Configuration.RabbitUid, NG2Configuration.RabbitPwd));

            bus.Publish(retrySyncRequest, NG2Configuration.RabbitTopic);
            bus.Dispose();
        }
    }
}
