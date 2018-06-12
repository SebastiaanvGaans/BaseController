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
        List<Room> rooms = new List<Room>();

        public Floor(string floorName, int amountOfRooms, ConnectionFactory factory)
        {
            for (int i = 1; i <= amountOfRooms; i++)
            {
                rooms.Add(new Room(floorName + "R" + i, factory));
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
