using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseController
{
    public class SensorViewModel : INotifyPropertyChanged
    {
        public SensorViewModel()
        {
            _Sensor = new Sensor();
            _Sensor.Name = "dave";
        }

        private Sensor _Sensor;

        public Sensor Sensor
        {
            get
            {
                return _Sensor;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
