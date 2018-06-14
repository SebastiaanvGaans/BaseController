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
    public class Controllable
    {
        MessageReciever reciever;
        MessageSender sender;

        public ControllableType type;
        public bool value;

        public string name;

        public Controllable(string name, ControllableType controllableType, ConnectionFactory connectionFactory)
        {
            reciever = new MessageReciever(connectionFactory);
            sender = new MessageSender(connectionFactory);

            type = controllableType;

            value = true;
            this.name = name;

            var consumer = new EventingBasicConsumer(reciever.GetChannel());
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);
                Command command = JsonConvert.DeserializeObject<Command>(message);

                switch (command.type)
                {
                    case CommandTypes.On:
                        this.UpdateValue(true, command);
                        break;
                    case CommandTypes.Off:
                        this.UpdateValue(false, command);
                        break;
                    case CommandTypes.Resend:
                        this.RequestControllableState(command);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("Invalid command: " + command.ToString());
                        break;
                }
            };
            List<string> routingKeys = new List<string>();
            routingKeys.Add(type.ToString());

            reciever.SetListenerToExchange(
                GateWayConfig.EXCHANGE_CONTROLLABLE_GENERAL,
                consumer,
                routingKeys);


            #region old RPC
            ///var channel = reciever.GetChannel();

            //channel.BasicQos(0, 1, false);

            //var consumer = new EventingBasicConsumer(channel);
            //consumer.Received += (model, ea) =>
            //{
            //    string response = null;

            //    var body = ea.Body;
            //    var props = ea.BasicProperties;
            //    var replyProps = channel.CreateBasicProperties();
            //    replyProps.CorrelationId = props.CorrelationId;

            //    try
            //    {
            //        var message = Encoding.UTF8.GetString(body);
            //        int n = int.Parse(message);
            //        Console.WriteLine(" [.] fib({0})", message);
            //        response = true.ToString();
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(" [.] " + e.Message);
            //        response = "";
            //    }
            //    finally
            //    {
            //        var responseBytes = Encoding.UTF8.GetBytes(response);
            //        channel.BasicPublish(
            //            exchange: "",
            //            routingKey: props.ReplyTo,
            //            basicProperties: replyProps,
            //            body: responseBytes);

            //        channel.BasicAck(
            //            deliveryTag: ea.DeliveryTag,
            //            multiple: false);
            //    }
            //};

            //List<string> routingKeys = new List<string>();
            ////routingKeys.Add();

            //reciever.SetListenerToQueue("test", consumer);
            ////(
            ////    GateWayConfig.EXCHANGE_SENSOR_CONTROL,
            ////    consumer,
            ////    routingKeys);

            //var factory = new ConnectionFactory() { HostName = GateWayConfig.HOST };
            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //    channel.QueueDeclare(queue: "test", durable: false,
            //      exclusive: false, autoDelete: false, arguments: null);
            //    channel.BasicQos(0, 1, false);
            //    var consumer = new EventingBasicConsumer(channel);

            //    consumer.Received += (model, ea) =>
            //    {
            //        string response = null;

            //        var body = ea.Body;
            //        var props = ea.BasicProperties;
            //        var replyProps = channel.CreateBasicProperties();
            //        replyProps.CorrelationId = props.CorrelationId;

            //        try
            //        {
            //            var message = Encoding.UTF8.GetString(body);
            //            int n = int.Parse(message);
            //            Console.WriteLine(" [.] fib({0})", message);
            //            response = "Succes";
            //        }
            //        catch (Exception e)
            //        {
            //            Console.WriteLine(" [.] " + e.Message);
            //            response = "";
            //        }
            //        finally
            //        {
            //            var responseBytes = Encoding.UTF8.GetBytes(response);
            //            channel.BasicPublish(
            //                exchange: "", 
            //                routingKey: props.ReplyTo,
            //                basicProperties: replyProps, 
            //                body: responseBytes);

            //            channel.BasicAck(deliveryTag: ea.DeliveryTag,
            //              multiple: false);
            //        }
            //    };

            //    channel.BasicConsume(queue: "test", autoAck: false, consumer: consumer);
            //    Console.WriteLine(" [x] Awaiting RPC requests");

            //}
            #endregion
        }

        public void UpdateValue(bool newVal, Command command = null)
        {
            this.value = newVal;

            //Send state to command answer queue
            RequestControllableState(command);

            //if (value) System.Diagnostics.Debug.WriteLine(name + ": Opened/Turned on " + type.ToString());
            //else System.Diagnostics.Debug.WriteLine(name + ": Closed/Turned off " + type.ToString());
        }

        public void RequestControllableState(Command command = null)
        {
            string message = name;
            string routingKey = command == null ? "reply" : command.returnKey;
            //TODO send state to command answer buss
            if (command == null)
            {
                if (value) message = name + ": " + type.ToString() + " is opened/turned on";
                else message = name + ": " + type.ToString() + " is closed/turned off ";
            }
            else
            {
                var slip = new ConfirmationSlip(
                    command.origin,
                    this.name,
                    this.type,
                    this.value);

                message = JsonConvert.SerializeObject(slip);
            }

            sender.SendToExchange(
                GateWayConfig.EXCHANGE_CONTROLLABLE_ANSWER,
                message,
                "direct",
                "reply");

        }


    }
}
