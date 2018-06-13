﻿using RabbitMQ.Client;
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
        List<Controllable> controllables;

        string fullName { get => baseName + "." + floorName + "." + roomName; }
        string baseName;
        string floorName;
        string roomName;

        public Room(string baseName, string floorName, string roomName, ConnectionFactory factory)
        {
            sender = new MessageSender(factory);

            sensors = new List<SensorUnit>();
            controllables = new List<Controllable>();

            this.baseName = baseName;
            this.floorName = floorName;
            this.roomName = roomName;

            foreach (SensorTypes type in Enum.GetValues(typeof(SensorTypes)))
                sensors.Add(new SensorUnit(factory, type));

            foreach (ControllableType type in Enum.GetValues(typeof(ControllableType)))
                controllables.Add(new Controllable(fullName, type, factory));

            reciever = new MessageReciever(factory);
            SetUpTopicListener();
        }

        private void SetUpTopicListener()
        {
            //Listen for sensor commands
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

            //Listen to controllable commands
            consumer = new EventingBasicConsumer(reciever.GetChannel());
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);
                Command command = JsonConvert.DeserializeObject<Command>(message);

                switch (command.type)
                {
                    case CommandTypes.On:
                        SetControllable(true, command.controllable);
                        break;
                    case CommandTypes.Off:
                        SetControllable(false, command.controllable);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("Invalid command: " + command.ToString());
                        break;
                }
            };
            routingKeys = new List<string>();
            routingKeys.Add(fullName);

            reciever.SetListenerToExchange(
                GateWayConfig.EXCHANGE_CONTROLLABLE_GENERAL,
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
                    GateWayConfig.QUEUE_SENSOR_IN);
            }

            sender.CloseConnection();
        }

        public void Change(SensorTypes sensor, float value)
        {
            Measurement measurement = sensors.Find(x => x.sensorType == sensor).ChangeValue(value);
        }

        public void SetControllable(bool value, ControllableType type)
        {
            controllables.Find(x => x.type == type).UpdateValue(value);
        }
    }
}
