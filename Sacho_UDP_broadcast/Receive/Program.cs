using BroadcastLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Receive
{
    class Program
    {
        static void Main(string[] args)
        {
            BroadcastListener listener = new BroadcastListener(8409, BoardcastMsgAnalysis.GetReply,null);
            listener.StartListening();

            Console.ReadKey();
        }
    }
}
