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
        public static int MaxRetryCount
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["MaxRetryCount"]);
            }
        }
    }
}
