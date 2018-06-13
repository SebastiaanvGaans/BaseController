
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace BaseController
{
    class SensorUnit
    {
        Random rand = new Random();

        MessageReciever reciever;

        SensorTypes sensorType;

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
            return new Measurement(value, sensorType);
        }

        public Measurement Update()
        {
            this.value += rand.Next(-5, 5);

            return new Measurement(value, sensorType);
        }
    }
}
