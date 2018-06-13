using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace BaseController
{
    public class Base
    {
        List<Floor> floors = new List<Floor>();

        string baseName;

        public Base(string baseName, int amountOfFloors, int amountOfRoomsPerFloor, ConnectionFactory factory)
        {
            this.baseName = baseName;

            for (int i = 1; i <= amountOfFloors; i++)
                floors.Add(new Floor(baseName, "F" + i, amountOfRoomsPerFloor, factory));

        }

        public void Update()
        {
            foreach (Floor floor in floors)
                floor.Update();
        }
    }
}
