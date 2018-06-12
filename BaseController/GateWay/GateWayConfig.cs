using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace BaseController
{
    public static class GateWayConfig
    {
        //public static string HOST = "192.168.24.77";
        public static string HOST = "localhost";

        private static ConnectionFactory factory = new ConnectionFactory { HostName = HOST };

        public static IConnection GetConnection()
        {
            return factory.CreateConnection();
        }

    }
}
