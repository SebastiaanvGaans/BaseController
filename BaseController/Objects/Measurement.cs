using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseController
{
    class Measurement
    {
        public string roomName;
        public float value;
        public SensorTypes type;

        public Measurement()
        {
        }
        public Measurement(string roomName, float value, SensorTypes type)
        {
            this.roomName = roomName;
            this.value = value;
            this.type = type;
        }

        public override string ToString()
        {
            return roomName + "; "+ type.ToString() + ": " + value;
        }
    }
}
