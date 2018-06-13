using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseController;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BaseControlCenter
{
    public class ControlCenter
    {
        public ConnectionFactory connectionFactory;

        MessageReciever reciever;
        MessageSender sender;

        public Dictionary<string, Measurement> measurements = new Dictionary<string, Measurement>();


        public ControlCenter()
        {
            connectionFactory = new ConnectionFactory { HostName = GateWayConfig.HOST }; ;

            reciever = new MessageReciever(connectionFactory);
            sender = new MessageSender(connectionFactory);

            SetUpReceivers();
        }

        private void SetUpReceivers()
        {
            EventingBasicConsumer consumer;
            List<string> routingKeys = new List<string>();


            //Listen to sensor inputs
            consumer = new EventingBasicConsumer(reciever.GetChannel());
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);
                this.HandleIncomingMeasurement(JsonConvert.DeserializeObject<Measurement>(message));
            };
            routingKeys.Clear();
            routingKeys.Add(GateWayConfig.QUEUE_SENSOR_IN);
            reciever.SetListenerToExchange(
                GateWayConfig.EXCHANGE_SENSOR_IN,
                consumer,
                routingKeys);

            //Listen to sensor answers
            consumer = new EventingBasicConsumer(reciever.GetChannel());
            consumer.Received += (model, ea) =>
            {
                //TODO currently unused

                var message = Encoding.UTF8.GetString(ea.Body);
                //this.HandleIncomingMeasurement(JsonConvert.DeserializeObject<Measurement>(message));
            };
            routingKeys.Clear();
            routingKeys.Add(GateWayConfig.QUEUE_SENSOR_ANSWER);
            reciever.SetListenerToExchange(
                GateWayConfig.EXCHANGE_SENSOR_IN,
                consumer,
                routingKeys);
        }

        private void HandleIncomingMeasurement(Measurement measurement)
        {
            if (80 < measurement.value && measurement.value < 120)
                measurements[measurement.ID] = measurement;
            else
            {
                Command command = new Command(CommandTypes.Change);
                command.sensor = measurement.type;
                command.value = 100;

                sender.SendToExchange(
                    GateWayConfig.EXCHANGE_SENSOR_CONTROL,
                    JsonConvert.SerializeObject(command),
                    "direct",
                    measurement.origin);
                System.Diagnostics.Debug.WriteLine("Sending command to: " + measurement.origin + " " + measurement.type);
            }
            System.Diagnostics.Debug.WriteLine(measurement.ToString());
        }

        public void updateSpecific(string routingKey, CommandTypes selectedCommandType)
        {
            Command command = new Command(selectedCommandType);

            sender.SendToExchange(
                    GateWayConfig.EXCHANGE_SENSOR_CONTROL,
                    JsonConvert.SerializeObject(command),
                    "direct",
                    routingKey);
        }

        public void ControllableCommandGeneral(CommandTypes commandType, ControllableType controllableType)
        {
            Command command = new Command(commandType);

            sender.SendToExchange(
                GateWayConfig.EXCHANGE_CONTROLLABLE_GENERAL,
                JsonConvert.SerializeObject(command),
                "direct",
                controllableType.ToString());
        }
        public void ControllableCommandSpecific(string routingKey, CommandTypes commandType, ControllableType controllableType)
        {
            Command command = new Command(commandType);

            sender.SendToExchange(
                GateWayConfig.EXCHANGE_CONTROLLABLE_GENERAL,
                JsonConvert.SerializeObject(command),
                "direct",
                routingKey);
        }
    }
}
