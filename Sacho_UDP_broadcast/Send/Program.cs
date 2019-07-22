using BroadcastLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Send
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;

            while (i < 3)
            {
                IPAddress broadcast = IPAddress.None;
                if (IPAddressQuery.GetBroadcastAddress(out broadcast))
                {
                    //broadcast = IPAddress.Broadcast;
                    BoardcastSender sender = new BoardcastSender(8409, broadcast);
                    sender.SendAsync("GetACalCoreService" + Global.MsgSplitter + "asasasa");
                }
                i++;
                Thread.Sleep(1000);
                Console.Write($"Send:{i}");
            }

            Console.ReadKey();
        }
    }
}
