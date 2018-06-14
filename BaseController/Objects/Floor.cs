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

        List<Section> sections = new List<Section>();

        public Floor(
            string baseName,
            string floorName,
            int amountOfSections,
            int amountOfRooms,
            ConnectionFactory factory)
        {
            this.baseName = baseName;
            this.floorName = floorName;

            for (int i = 1; i <= amountOfSections; i++)
            {
                sections.Add(new Section(baseName, floorName, "S" + i, amountOfRooms, factory));
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
            foreach (Section section in this.sections)
                section.Update();
            //room.Update();
        }

        public void Resend()
        {
            foreach (Section section in this.sections)
                section.Resend();
        }

        public void SetControllable(Command command)
        {
            sections.ForEach(x => x.SetControllable(command));
        }

        public void RequestControllableState(Command command)
        {
            sections.ForEach(x => x.RequestControllableState(command));
        }
    }
}
