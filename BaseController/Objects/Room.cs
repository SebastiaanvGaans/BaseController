using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BaseController
{
    public class Room
    {
        MessageSender sender;

        List<SensorUnit> sensors;

        string fullName { get => baseName + "." + floorName + "." + roomName; }
        string baseName;
        string floorName;
        string roomName;

        public Room(string baseName, string floorName,  string roomName, ConnectionFactory factory)
        {
            sender = new MessageSender(factory);

            sensors = new List<SensorUnit>();

            this.baseName = baseName;
            this.floorName = floorName;
            this.roomName = roomName;
            
            foreach (SensorTypes type in Enum.GetValues(typeof(SensorTypes)))
                sensors.Add(new SensorUnit(factory, type));


        }


        public void Update()
        {
            //foreach (SensorUnit sensor in sensors)
            //    new Thread(() => sensor.Update()).Start();

            sender.OpenConnection();

            foreach (SensorUnit sensor in sensors)
            {
                Measurement measurement = sensor.Update();
                measurement.name = fullName;

                sender.SendToExchange(
                    GateWayConfig.EXCHANGE_SENSOR_IN,
                    JsonConvert.SerializeObject(measurement),
                    "direct",
                    GateWayConfig.QUEUE_SENSOR_IN);
            }
            sender.CloseConnection();
                
        }
    }
}
