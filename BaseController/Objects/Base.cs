using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BaseController
{
    public class Base
    {
        MessageReciever reciever;

        string fullName { get => baseName; }
        string baseName;

        List<Floor> floors = new List<Floor>();

        public Base(
            string baseName,
            int amountOfFloors,
            int amountOfRoomsPerFloor,
            ConnectionFactory factory)
        {
            this.baseName = baseName;

            for (int i = 1; i <= amountOfFloors; i++)
                floors.Add(new Floor(baseName, "F" + i, amountOfRoomsPerFloor, factory));

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
        }

        public void Update()
        {
            foreach (Floor floor in floors)
                floor.Update();
        }

        public void Resend()
        {
            foreach (Floor floor in floors)
                floor.Resend();
        }
    }
}
