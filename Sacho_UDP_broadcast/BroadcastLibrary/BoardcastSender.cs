using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BroadcastLibrary
{
    public class BoardcastSender
    {
        /// <summary>
        /// 广播地址
        /// </summary>
        public IPAddress BroadcastAddress { get; set; }

        /// <summary>
        /// 广播的端口
        /// </summary>
        public int BroadcastPort { get; set; }

        /// <summary>
        /// UDP广播发送者
        /// </summary>
        UdpClient udpSender { get; set; }

        /// <summary>
        /// 发送线程
        /// </summary>
        Thread sendThread { get; set; }

        public BoardcastSender(int sendPort,IPAddress broadcastAddress)
        {
            BroadcastPort = sendPort;
            BroadcastAddress = broadcastAddress;
            IPAddress ip = IPAddress.None;
            if (IPAddressQuery.GetLocalIP(out ip))
            {
                IPEndPoint iep = new IPEndPoint(ip, BroadcastPort);
                udpSender = new UdpClient(iep);
            }
            else
            {
                IPEndPoint iep = new IPEndPoint(BroadcastAddress, BroadcastPort);
                udpSender = new UdpClient(iep);
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="content"></param>
        public void SendAsync(string content)
        {
            if (sendThread != null && sendThread.IsAlive)
                return;

            sendThread = new Thread(() => {
                byte[] send = System.Text.Encoding.UTF8.GetBytes(content);
                udpSender.Send(send, send.Length, new IPEndPoint(BroadcastAddress, BroadcastPort));
            });
            sendThread.IsBackground = true;
            sendThread.Start();
        }



    }
}
