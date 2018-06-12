using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaseController
{
    public class Room
    {
        MessageSender sender;

        List<SensorUnit> sensors;
        string roomName;

        public Room(string roomName, ConnectionFactory factory)
        {
            sensors = new List<SensorUnit>();
            this.roomName = roomName;

            foreach (SensorTypes type in Enum.GetValues(typeof(SensorTypes)))
                sensors.Add(new SensorUnit(factory, this.roomName, type));

            sender = new MessageSender(factory);
        }


        public void Update()
        {
            //foreach (SensorUnit sensor in sensors)
            //    new Thread(() => sensor.Update()).Start();

            sender.OpenConnection();

            foreach (SensorUnit sensor in sensors)
                sender.SendToQueue("SebastiaansTestQueue", sensor.Update());

            sender.CloseConnection();
                
        }
    }
}
