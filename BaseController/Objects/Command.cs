using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseController
{
    public class Command
    {
        public CommandTypes type;

        public SensorTypes sensor;
        public float value;

        public ControllableType controllable;

        public Command(CommandTypes type)
        {
            this.type = type;
        }

        public override string ToString()
        {
            if (type == CommandTypes.Change)
                return type.ToString() + ", " + sensor.ToString() + ";" + value;
            else
                return type.ToString();
        }
    }
}
