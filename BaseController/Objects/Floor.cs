using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaseController
{
    public class Floor
    {
        string baseName;
        string floorName;

        List<Room> rooms = new List<Room>();

        public Floor(string baseName,string floorName, int amountOfRooms, ConnectionFactory factory)
        {
            this.baseName = baseName;
            this.floorName = floorName;

            for (int i = 1; i <= amountOfRooms; i++)
            {
                rooms.Add(new Room(baseName, floorName, "R" + i, factory));
            }
        }

        public void Update()
        {
            foreach (Room room in this.rooms)
                new Thread(() => room.Update()).Start();
            //room.Update();
        }
    }
}
