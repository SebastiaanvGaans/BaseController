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
        Base _base;

        public MainWindow()
        {
            InitializeComponent();
            _base = new Base("Stargate", 5, 5, factory);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _base.Update();
        }
    }
}
