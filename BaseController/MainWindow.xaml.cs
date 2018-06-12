using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;

namespace BaseController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ConnectionFactory factory = new ConnectionFactory { HostName = GateWayConfig.HOST};
        Base _base;

        public MainWindow()
        {
            InitializeComponent();
            _base = new Base("Stargate", 5, 50, factory);

            var messageReciever = new MessageReciever(factory);
            var consumer = new EventingBasicConsumer(messageReciever.GetChannel());
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                Measurement measurement = JsonConvert.DeserializeObject<Measurement>(message);

                System.Diagnostics.Debug.WriteLine(measurement.ToString());
            };

            messageReciever.SetListenerToQueue("SebastiaansTestQueue", consumer);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            

            

            _base.Update();


            //var messageSender = new MessageSender();
            //messageSender.SendToChannel("SebastiaansTestQueue", "HELLO");
            //messageSender.SendToExchange("Test", "HELLO");

            //var factory = new ConnectionFactory() { HostName = "192.168.24.77" };
            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //    channel.QueueDeclare(queue: "hello",
            //                         durable: false,
            //                         exclusive: false,
            //                         autoDelete: false,
            //                         arguments: null);

            //    string message = "Hello World!";
            //    var body = Encoding.UTF8.GetBytes(message);

            //    channel.BasicPublish(exchange: "",
            //                         routingKey: "hello",
            //                         basicProperties: null,
            //                         body: body);
            //     System.Diagnostics.Debug.WriteLine(" [x] Sent {0}", message);
            //}

        }
    }
}
