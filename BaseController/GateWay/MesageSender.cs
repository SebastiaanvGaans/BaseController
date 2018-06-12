using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace BaseController
{
    public class MessageSender
    {
        //private string HOST = "192.168.24.77";

        private ConnectionFactory factory = null;
        private IConnection connection = null;
        private IModel channel = null;

        public MessageSender(ConnectionFactory factory)
        {
            ///this.factory = new ConnectionFactory { HostName = this.HOST };

            this.factory = factory;

        }

        public void OpenConnection()
        {
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public void CloseConnection()
        {
            connection.Close();
        }

        public void SendToQueue(string queueName, string message)
        {
            if (connection == null)
                OpenConnection();

            channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(message));
        }

        public void SendToExchange(
            string exchangeName,
            string message,
            string exchangeType = "direct",
            string routingKey = "")
        {
            if (connection == null)
                OpenConnection();

            channel.ExchangeDeclare(
                exchange: exchangeName,
                type: exchangeType);

            channel.BasicPublish(
                exchange: exchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(message));
            connection.Close();
        }

    }
}
