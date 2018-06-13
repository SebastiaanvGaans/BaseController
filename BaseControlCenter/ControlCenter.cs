using System;
using System.Collections.Generic;
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
        ConnectionFactory connectionFactory;

        MessageReciever reciever;
        MessageSender sender;

        Dictionary<string, Measurement> measurements = new Dictionary<string, Measurement>();

        public ControlCenter()
        {
            connectionFactory = new ConnectionFactory { HostName = GateWayConfig.HOST }; ;

            reciever = new MessageReciever(connectionFactory);
            sender = new MessageSender(connectionFactory);

            SetUpReceivers();
        }

        private void SetUpReceivers()
        {
            var consumer = new EventingBasicConsumer(reciever.GetChannel());
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);
                this.HandleIncomingMeasurement(JsonConvert.DeserializeObject<Measurement>(message));                
            };
            List<string> routingKeys = new List<string>();
            routingKeys.Add(GateWayConfig.QUEUE_SENSOR_IN);
            reciever.SetListenerToExchange(
                GateWayConfig.EXCHANGE_SENSOR_IN,
                consumer,
                routingKeys);
        }

        private void HandleIncomingMeasurement(Measurement measurement)
        {
            if( 80 < measurement.value && measurement.value < 120)
                measurements[measurement.ID] = measurement;
            else
            {

            }

            System.Diagnostics.Debug.WriteLine(measurement.ToString());
        }
    }
}
