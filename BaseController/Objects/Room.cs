using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace BaseController
{
    public class Room
    {
        MessageSender sender;
        MessageReciever reciever;

        List<SensorUnit> sensors;

        string fullName { get => baseName + "." + floorName + "." + roomName; }
        string baseName;
        string floorName;
        string roomName;

        public Room(string baseName, string floorName, string roomName, ConnectionFactory factory)
        {
            sender = new MessageSender(factory);

            sensors = new List<SensorUnit>();

            this.baseName = baseName;
            this.floorName = floorName;
            this.roomName = roomName;

            foreach (SensorTypes type in Enum.GetValues(typeof(SensorTypes)))
                sensors.Add(new SensorUnit(factory, type));

            reciever = new MessageReciever(factory);
            SetUpTopicListener();
        }

        private void SetUpTopicListener()
        {
            var consumer = new EventingBasicConsumer(reciever.GetChannel());
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);
                Command command = JsonConvert.DeserializeObject<Command>(message);

                switch (command.type)
                {
                    case CommandTypes.Update:
                        this.Update();
                        break;
                    case CommandTypes.Resend:
                        this.Resend();
                        break;
                    case CommandTypes.Change:
                        this.Change(command.sensor, command.value);
                        break;
                    case CommandTypes.None:
                    default:
                        System.Diagnostics.Debug.WriteLine("Invalid command: " + command.ToString());
                        break;
                }
            };
            List<string> routingKeys = new List<string>();
            routingKeys.Add(fullName);

            reciever.SetListenerToExchange(
                GateWayConfig.EXCHANGE_SENSOR_CONTROL,
                consumer,
                routingKeys);
        }

        public void Update()
        {
            sender.OpenConnection();

            foreach (SensorUnit sensor in sensors)
            {
                Measurement measurement = sensor.Update();
                measurement.origin = fullName;

                sender.SendToExchange(
                    GateWayConfig.EXCHANGE_SENSOR_IN,
                    JsonConvert.SerializeObject(measurement),
                    "direct",
                    GateWayConfig.QUEUE_SENSOR_IN);
            }

            sender.CloseConnection();
        }

        public void Resend()
        {
            sender.OpenConnection();

            foreach (SensorUnit sensor in sensors)
            {
                Measurement measurement = sensor.GetMeasurement();
                measurement.origin = fullName;

                sender.SendToExchange(
                    GateWayConfig.EXCHANGE_SENSOR_IN,
                    JsonConvert.SerializeObject(measurement),
                    "direct",
                    GateWayConfig.QUEUE_SENSOR_ANSWER);
            }

            sender.CloseConnection();
        }

        public void Change(SensorTypes sensor, float value)
        {
            Measurement measurement = sensors.Find(x => x.sensorType == sensor).ChangeValue(value);
        }
    }
}
