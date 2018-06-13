
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BaseController
{
    class SensorUnit
    {
        Random rand = new Random();

        MessageReciever reciever;

        public SensorTypes sensorType;

        float value = 100;

        public SensorUnit(ConnectionFactory factory, SensorTypes type)
        {
            this.sensorType = type;
            reciever = new MessageReciever(factory);
            //sender = new MessageSender(factory);
        }

        public Measurement ChangeValue(float newValue)
        {
            this.value = newValue;
            return GetMeasurement();
        }

        public Measurement Update()
        {
            this.value += rand.Next(-5, 5);

            return GetMeasurement();
        }

        public Measurement GetMeasurement()
        {
            return new Measurement(value, sensorType);
        }
    }
}
