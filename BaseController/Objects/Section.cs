using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BaseController
{
    public class Section
    {
        MessageReciever reciever;

        string fullName { get => baseName + "." + floorName + "." + sectionName; }
        string baseName;
        string floorName;
        string sectionName;

        public List<Room> rooms = new List<Room>();

        public Section(
            string baseName,
            string floorName,
            string sectionName,
            int amountOfRooms,
            ConnectionFactory factory)
        {
            this.baseName = baseName;
            this.floorName = floorName;
            this.sectionName = sectionName;

            for (int i = 1; i <= amountOfRooms; i++)
            {
                rooms.Add(new Room(baseName, floorName, sectionName, "R" + i, factory));
            }

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
                        SetControllable(command);
                        break;
                    case CommandTypes.Off:
                        SetControllable(command);
                        break;
                    case CommandTypes.Resend:
                        RequestControllableState(command);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("Invalid command: " + command.ToString());
                        break;
                }
            };
            routingKeys = new List<string>();
            routingKeys.Add(fullName);

            reciever.SetListenerToExchange(
                GateWayConfig.EXCHANGE_CONTROLLABLE_SPECIFIC,
                consumer,
                routingKeys);
        }

        public void Update()
        {
            foreach (Room room in this.rooms)
                new Thread(() => room.Update()).Start();
            //room.Update();
        }

        public void Resend()
        {
            foreach (Room room in this.rooms)
                new Thread(() => room.Resend()).Start();
        }

        public void SetControllable(Command command)
        {
            rooms.ForEach(x => x.SetControllable(command));
        }

        public void RequestControllableState(Command command)
        {
            rooms.ForEach(x => x.RequestControllableState(command));
        }
    }
}
