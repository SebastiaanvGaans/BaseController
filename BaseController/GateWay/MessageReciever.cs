using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BaseController
{
    public class MessageReciever
    {
        //private string HOST = "192.168.24.77";

        //private ConnectionFactory factory = null;
        private IConnection connection = null;
        private IModel channel = null;

        public MessageReciever(ConnectionFactory factory)
        {
            //this.factory = new ConnectionFactory { HostName = this.HOST };

            connection = factory.CreateConnection(); ;
            channel = connection.CreateModel();
        }

        public IModel GetChannel()
        {
            return this.channel;
        }

        public void SetListenerToQueue(
            string queueName,
            EventingBasicConsumer consumer)
        {
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }

        public void SetListenerToExchange(
            string exchangeName,
            EventingBasicConsumer consumer,
            string exchangeType = "direct")
        {
            channel.ExchangeDeclare(
                exchange: exchangeName,
                type: exchangeType);

            string queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName,
                              exchange: exchangeName,
                              routingKey: "");
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

        }

        public void SetListenerToExchange(
            string exchangeName,
            EventingBasicConsumer consumer,
            List<string> routingKeys,
            string exchangeType = "direct")
        {
            channel.ExchangeDeclare(
                exchange: exchangeName,
                type: exchangeType);

            string queueName = channel.QueueDeclare().QueueName;

            foreach (string key in routingKeys)
                channel.QueueBind(queue: queueName,
                                  exchange: exchangeName,
                                  routingKey: key);
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

        }
    }
}
