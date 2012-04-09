using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TSP
{
    class Miner
    {
        public int repeats { get; private set; }

        public Miner()
        {
            repeats = 0;
        }

        public void Sing()
        {
            Console.WriteLine("[MINER] I will perform a classic piece from Snow White!");
            while (true)
            {
                Thread.Sleep(2000);
                Console.WriteLine("Hi Hoooo!");
                ++repeats;
            }
        }
    }
}
