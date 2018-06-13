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


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string key = Base.Text + "." + Floor.Text + "." + Room.Text;

            control.updateSpecific(key, (CommandTypes)commandtypes.SelectedItem);
        }

        private void UpdateListView(object sender, RoutedEventArgs e)
        {
            var items = from pair in control.measurements orderby pair.Key ascending select pair;
            //items.ToList();
            //currentData.ItemsSource = control.measurements.OrderBy(i => i.Key).Values.ToList();
            currentData.ItemsSource = items.ToList();
        }


    }
}
