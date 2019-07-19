using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sacho_UDP_broadcast
{
    class Program
    {
        static void Main(string[] args)
        {

            var x = IPAddress.Any;


            MyBroadcast mst = new MyBroadcast();

            Thread a = new Thread(mst.receiveBroadcast);
            a.IsBackground = true;
            a.Start();

            //Console.Write("send" + Environment.NewLine);
            while (true)
            {
                string msg = Console.ReadLine();
                if (!string.IsNullOrEmpty(msg))
                {
                    mst.sendBroadcast(msg, 8409);
                }
            }

        }
    }
}
