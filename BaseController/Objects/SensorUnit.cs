using Newtonsoft.Json;
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
        //MessageSender sender;

        SensorTypes sensorType;

        string roomName;

        float value = 100;

        public SensorUnit(ConnectionFactory factory, string roomName, SensorTypes type)
        {
            this.roomName = roomName;
            this.sensorType = type;
            reciever = new MessageReciever(factory);
            //sender = new MessageSender(factory);
        }

        public void SendMeasurement()
        {
            Measurement measurement = new Measurement(this.roomName, this.value, this.sensorType);
            string message = JsonConvert.SerializeObject(measurement);

            //Measurement measurement = JsonConvert.DeserializeObject<Measurement>(message);
            //string message = "It's " + value + "Degree in room" + roomName;
            //sender.SendToQueue("SebastiaansTestQueue", message);
        }

        public string Update()
        {

            this.value += rand.Next(-5, 5);

            Measurement measurement = new Measurement(this.roomName, this.value, this.sensorType);
            return JsonConvert.SerializeObject(measurement);


            //Send new measurement
            //this.SendMeasurement();
        }
    }
}
