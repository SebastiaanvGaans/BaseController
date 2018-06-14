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
        ConnectionFactory factory = new ConnectionFactory { HostName = GateWayConfig.HOST };
        Base Stargate;
        Base Area51;

        public MainWindow()
        {
            InitializeComponent();
            Stargate = new Base("Stargate", 4, 4, 4, factory);
            //Area51 = new Base("Area51", 4, 2, 2, factory);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Stargate.Update();
            //Area51.Update();
        }
    }
}
