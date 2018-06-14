using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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

        List<string> expected = new List<string>();

        public List<string> unreturnedCalls = new List<string>();

        public Dictionary<string, Measurement> measurements = new Dictionary<string, Measurement>();
        public Dictionary<DateTime, Measurement> badMeasurements = new Dictionary<DateTime, Measurement>();


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

            //Listen to Controllable answers
            consumer = new EventingBasicConsumer(reciever.GetChannel());
            consumer.Received += (model, ea) =>
            {
                //TODO currently unused

                var message = Encoding.UTF8.GetString(ea.Body);
                HandleCommandAnswer(JsonConvert.DeserializeObject<ConfirmationSlip>(message));
                
                //System.Diagnostics.Debug.WriteLine(message);
                //HandleCommandAnswer(JsonConvert.DeserializeObjec<>());
                
            };
            routingKeys.Clear();
            routingKeys.Add("reply");
            reciever.SetListenerToExchange(
                GateWayConfig.EXCHANGE_CONTROLLABLE_ANSWER,
                consumer,
                routingKeys);
        }

        private void HandleIncomingMeasurement(Measurement measurement)
        {
            if (80 < measurement.value && measurement.value < 120)
                measurements[measurement.ID] = measurement;
            else
            {
                badMeasurements.Add(DateTime.Now, measurement);
                ChangeSpecific(measurement, 100);
                System.Diagnostics.Debug.WriteLine("Sending command to: " + measurement.origin + " " + measurement.type);
            }
            System.Diagnostics.Debug.WriteLine(measurement.ToString());
        }

        public void UpdateSpecific(string routingKey, CommandTypes selectedCommandType)
        {
            //System.Diagnostics.Debug.WriteLine(routingKey);
            Command command = new Command(selectedCommandType);

            sender.SendToExchange(
                    GateWayConfig.EXCHANGE_SENSOR_CONTROL,
                    JsonConvert.SerializeObject(command),
                    "direct",
                    routingKey);
        }

        public void ChangeSpecific(Measurement measurement, float value)
        {
            Command command = new Command(CommandTypes.Change);
            command.sensor = measurement.type;
            command.value = value;

            sender.SendToExchange(
                GateWayConfig.EXCHANGE_SENSOR_CONTROL,
                JsonConvert.SerializeObject(command),
                "direct",
                measurement.origin);
        }

        public void ControllableCommandGeneral(CommandTypes commandType, ControllableType controllableType)
        {
            Command command = new Command(commandType);
            command.origin = controllableType.ToString();

            expected.Add(command.origin);

            sender.SendToExchange(
                GateWayConfig.EXCHANGE_CONTROLLABLE_GENERAL,
                JsonConvert.SerializeObject(command),
                "direct",
                controllableType.ToString());


        }
        public void ControllableCommandSpecific(string routingKey, CommandTypes commandType, ControllableType controllableType)
        {
            Command command = new Command(commandType);
            command.origin = routingKey;
            command.controllable = controllableType;

            expected.Add(command.origin);

            sender.SendToExchange(
                GateWayConfig.EXCHANGE_CONTROLLABLE_SPECIFIC,
                JsonConvert.SerializeObject(command),
                "direct",
                routingKey);

            new Thread(() => WaitForAnswer(command.origin, 5000)).Start();
        }

        public void HandleCommandAnswer(ConfirmationSlip slip)
        {
            if (expected.Contains(slip.requestor))
                expected.Remove(slip.requestor);
            System.Diagnostics.Debug.WriteLine(slip.ToString());
        }

        public void WaitForAnswer(string key, int time)
        {
            Thread.Sleep(time);
            if (expected.Contains(key))
                unreturnedCalls.Add("No response on: " + key);
                System.Diagnostics.Debug.WriteLine("No response on: " + key);
        }
    }
}
