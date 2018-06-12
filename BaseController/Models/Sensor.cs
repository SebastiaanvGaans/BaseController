using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseController
{
    /// <summary>
    /// A single sensor
    /// </summary>
    public class Sensor
    {
        public Sensor()
        {

        }

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }


    }
}
