using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseController
{
    public class Measurement
    {
        public string ID { get => origin + type.ToString(); }
        public string origin;
        public float value;
        public SensorTypes type;

        public Measurement()
        {
        }
        public Measurement(float value, SensorTypes type)
        {
            this.value = value;
            this.type = type;

        }

        public override string ToString()
        {
            return origin + ": " + type.ToString() + " = " + value;
        }

        
    }
}
