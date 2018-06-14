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

        public MainWindow()
        {
            InitializeComponent();

            control = new ControlCenter();
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
            //System.Diagnostics.Debug.WriteLine(RoutingKey1.Text);
            //System.Diagnostics.Debug.WriteLine(commandtypes.SelectedItem);
            if (!"".Equals(RoutingKey1.Text) && commandtypes.SelectedItem != null)
            {
                control.UpdateSpecific(RoutingKey1.Text, (CommandTypes)commandtypes.SelectedItem);
                Thread.Sleep(500);
            }
        }

        private void UpdateListViews(object sender, RoutedEventArgs e)
        {
            var items = from pair in control.measurements orderby pair.Key ascending select pair;
            currentData.ItemsSource = items.ToList();

            var errors = from pair in control.badMeasurements orderby pair.Key ascending select pair;
            currentErrors.ItemsSource = new List<string>();
            currentErrors.ItemsSource = errors.ToList();

            currentUnreturned.ItemsSource = control.unreturnedCalls;
        }

        private void ControllableCommand(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(commandtypes2.SelectedItem);
            //System.Diagnostics.Debug.WriteLine(controllableType.SelectedItem);
            if (controllableType.SelectedItem != null && commandtypes2.SelectedItem != null)
            {
                control.ControllableCommandGeneral((CommandTypes)commandtypes2.SelectedItem, (ControllableType)controllableType.SelectedItem);
                Thread.Sleep(500);
            }
        }

        private void ControllableCommandSpecific(object sender, RoutedEventArgs e)
        {
            //if (!"".Equals(controllableType2.SelectedItem) && !"".Equals(commandtypes3.SelectedItem) && !"".Equals(RoutingKey2.Text))
            if (controllableType2.SelectedItem != null && commandtypes3.SelectedItem != null && !"".Equals(RoutingKey2.Text))
            {
                control.ControllableCommandSpecific(
                    RoutingKey2.Text,
                    (CommandTypes)commandtypes3.SelectedItem,
                    (ControllableType)controllableType2.SelectedItem);
                Thread.Sleep(500);
            }
        }

        private void Test(object sender, RoutedEventArgs e)
        {
            var rpcClient = new RpcClient();

            Console.WriteLine(" [x] Requesting fib(30)");
            var response = rpcClient.Call("30");
            Console.WriteLine(" [.] Got '{0}'", response);

            rpcClient.Close();
        }

        private void RefreshSelected(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(currentData.SelectedItem.ToString());
            Measurement measurement = ((KeyValuePair<string, Measurement>)currentData.SelectedItem).Value;

            control.UpdateSpecific(measurement.origin, CommandTypes.Resend);
        }
        private void UpdateSelected(object sender, RoutedEventArgs e)
        {
            Measurement measurement = ((KeyValuePair<string, Measurement>)currentData.SelectedItem).Value;

            control.UpdateSpecific(measurement.origin, CommandTypes.Update);
        }
        private void ChangeSelected(object sender, RoutedEventArgs e)
        {
            Measurement measurement = ((KeyValuePair<string, Measurement>)currentData.SelectedItem).Value;

            control.ChangeSpecific(measurement, float.Parse(ChangeValue.Text));
        }
    }
}
