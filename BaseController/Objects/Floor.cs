using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaseController
{
    public class Floor
    {
        MessageReciever reciever;

        string fullName { get => baseName + "." + floorName; }
        string baseName;
        string floorName;

        List<Room> rooms = new List<Room>();

        public Floor(
            string baseName,
            string floorName,
            int amountOfRooms,
            ConnectionFactory factory)
        {
            this.baseName = baseName;
            this.floorName = floorName;

            for (int i = 1; i <= amountOfRooms; i++)
            {
                rooms.Add(new Room(baseName, floorName, "R" + i, factory));
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
                        rooms.ForEach(x => x.SetControllable(true, command.controllable));
                        break;
                    case CommandTypes.Off:
                        rooms.ForEach(x => x.SetControllable(false, command.controllable));
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
            foreach (Room room in this.rooms)
                new Thread(() => room.Update()).Start();
            //room.Update();
        }

        public void Resend()
        {
            foreach (Room room in this.rooms)
                new Thread(() => room.Resend()).Start();
        }
    }
}
