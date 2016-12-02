using Fanex.NG2.ServiceBroker.Configurations;
using ONEbook.NG2.API.DTO.CreateAccount;

namespace Fanex.NG2.ServiceBroker.Models
{
    public class RetrySyncCustomerMessage
    {
        public RetrySyncCustomerMessage()
        {
        }

        public RetrySyncCustomerMessage(Request request)
        {
            Request = request;
        }

        public Request Request { get; set; }

        public int RetryCount { get; set; }

        public bool IsExceedMaxRetryCount()
        {
            return RetryCount >= NG2ServiceBrokerConfig.MaxRetryCount;
        }
    }
}
