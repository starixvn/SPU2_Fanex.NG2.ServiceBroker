using EasyNetQ;
using Fanex.Data.Configuration;
using Fanex.Module.NG2.Services;
using Fanex.NG2.ServiceBroker.Logging;
using Fanex.NG2.ServiceBroker.Models;
using ONEbook.NG2.API.DTO.CreateAccount;
using System;
using System.Collections.Generic;
using System.ServiceProcess;

namespace Fanex.NG2.ServiceBroker
{
    public partial class NG2ServiceBroker : ServiceBase
    {
        private IBus _bus;

        private string _retrySyncCustomerSubscriptionId = "Fanex.NG2.ServiceBroker.RetrySyncCustomers";

        public NG2ServiceBroker()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var NG2Configuration = ONEbook.NG2.API.Configuration.NG2Configuration.Instance;
            DbSettingProviderManager.Current.Start();
            _bus = RabbitHutch.CreateBus(
                  string.Format("host={0};virtualHost={1};username={2};password={3}", NG2Configuration.RabbitHost, NG2Configuration.RabbitName, NG2Configuration.RabbitUid, NG2Configuration.RabbitPwd),
                  serviceRegister =>
                  {
                      serviceRegister.Register<IEasyNetQLogger>(_ => new NG2ServiceBrokerLogger());
                  });

            _bus.Subscribe<Request>(
                NG2Configuration.RabbitTopic,
                syncRequest =>
                {
                    RetrySyncCustomer(new RetrySyncCustomerMessage(syncRequest));
                },
                config => config.WithTopic(NG2Configuration.RabbitTopic));

            _bus.Subscribe<RetrySyncCustomerMessage>(
                this._retrySyncCustomerSubscriptionId,
                syncRequest =>
                {
                    RetrySyncCustomer(syncRequest);
                });
        }

        protected override void OnStop()
        {
            _bus.Dispose();
        }

        private void RetrySyncCustomer(RetrySyncCustomerMessage request)
        {
            bool retryResult = false;
            var customerArray = new List<long>();
            foreach (var item in request.Request.Call.AccountArray)
            {
                customerArray.Add(item.ObCustId);
            }
            var customersList = string.Join(", ", customerArray);
            try
            {
                ING2StatusService service = new NG2StatusService();
                var result = service.RetrySyncCustomers(request.Request);
                retryResult = result.IsSuccess;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("Retry({0}) for customers: {1}.", request.RetryCount, customersList));
                Console.WriteLine(string.Format(""));
                Console.WriteLine("Failed.");
                Console.ResetColor();
                Fanex.Logging.Logger.Log.Error("Error from Onebook.NG2.API:" + ex.Message, ex);
            }

            Console.WriteLine();
            if (!request.IsExceedMaxRetryCount())
            {
                if (!retryResult)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(string.Format("Retry({0}) for customers: {1}.", request.RetryCount, customersList));
                    Console.WriteLine("IsSuccess = " + retryResult.ToString());
                    Console.WriteLine("Failed.");
                    Console.ResetColor();
                    Fanex.Logging.Logger.Log.Info(string.Format("Retry({0}) for customers: {1}. Result: {2}", request.RetryCount, customersList, retryResult ? "Success" : "Fail"));
                    request.RetryCount++;
                    _bus.Publish<RetrySyncCustomerMessage>(request);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(string.Format("Retry({0}) for customers: {1}", request.RetryCount, customersList));
                    Console.WriteLine("Successfully.");
                    Fanex.Logging.Logger.Log.Info(string.Format("Retry({0}) for customers: {1}. Result: {2}", request.RetryCount, customersList, retryResult ? "Success" : "Fail"));
                    Console.ResetColor();
                }
            }
        }
    }
}
