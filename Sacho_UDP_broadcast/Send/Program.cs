using BroadcastLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Send
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress broadcast = IPAddress.None;
            if (IPAddressQuery.GetBroadcastAddress(out broadcast))
            {
                BoardcastSender sender = new BoardcastSender(8408, broadcast);
                sender.SendAsync("GetACalCoreService" + Global.MsgSplitter + "asasasa");
            }
        }
    }
}
