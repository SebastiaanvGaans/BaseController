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

        public static string EXCHANGE_SENSOR_IN = "SensorIn";
        public static string EXCHANGE_SENSOR_CONTROL = "SensorControl";

        public static string QUEUE_SENSOR_IN = "SensorInputQueue";
        public static string QUEUE_SENSOR_ANSWER = "SensorAnswerQueue";

        public static string EXCHANGE_CONTROLLABLE_GENERAL = "GeneralControl";

    }
}
