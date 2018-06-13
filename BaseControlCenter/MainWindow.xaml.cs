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
using BaseController;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;

namespace BaseControlCenter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ControlCenter control;

        RPCClient rpcClient;

        public MainWindow()
        {
            InitializeComponent();

            control = new ControlCenter();
            rpcClient = new RPCClient(control.connectionFactory);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            commandtypes.ItemsSource = Enum.GetValues(typeof(CommandTypes)).Cast<CommandTypes>();
            commandtypes2.ItemsSource = Enum.GetValues(typeof(CommandTypes)).Cast<CommandTypes>();
            commandtypes3.ItemsSource = Enum.GetValues(typeof(CommandTypes)).Cast<CommandTypes>();
            controllableType.ItemsSource = Enum.GetValues(typeof(ControllableType)).Cast<ControllableType>();
            controllableType2.ItemsSource = Enum.GetValues(typeof(ControllableType)).Cast<ControllableType>();
        }

        private void SendCommand(object sender, RoutedEventArgs e)
        {
            control.updateSpecific(RoutingKey1.Text, (CommandTypes)commandtypes.SelectedItem);
        }

        private void UpdateListView(object sender, RoutedEventArgs e)
        {
            var items = from pair in control.measurements orderby pair.Key ascending select pair;
            //items.ToList();
            //currentData.ItemsSource = control.measurements.OrderBy(i => i.Key).Values.ToList();
            currentData.ItemsSource = items.ToList();
        }

        private void ControllableCommand(object sender, RoutedEventArgs e)
        {
            control.ControllableCommandGeneral((CommandTypes)commandtypes2.SelectedItem, (ControllableType)controllableType.SelectedItem);
        }

        private void ControllableCommandSpecific(object sender, RoutedEventArgs e)
        {
           control.ControllableCommandSpecific(RoutingKey2.Text, (CommandTypes)commandtypes3.SelectedItem, (ControllableType)controllableType2.SelectedItem);
        }
    }
}
