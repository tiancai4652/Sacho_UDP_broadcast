using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BroadcastLibrary
{
    public class BroadcastListener
    {
        /// <summary>
        /// 监听端口
        /// </summary>
        int listeningPort { get; set; }

        /// <summary>
        /// 监听线程
        /// </summary>
        Thread listenerThread { get; set; }

        /// <summary>
        /// UDP消息接收客户端
        /// </summary>
        UdpClient udpListening;

        /// <summary>
        /// 解析方法
        /// </summary>
        public Func<byte[],string> AnalyzeFunc { get; set; }

        /// <summary>
        /// 回调方法
        /// </summary>
        public Func<string, string> CallBackFunc { get; set; }

        public BroadcastListener(int listeningPort, Func<byte[], string> analyzeFunc =null, Func<string, string> callBackFunc=null)
        {
            this.listeningPort = listeningPort;
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, this.listeningPort);
            udpListening = new UdpClient(iep);
            AnalyzeFunc = analyzeFunc;
            CallBackFunc = callBackFunc;
        }

        public void StartListening()
        {
            if (listenerThread != null && listenerThread.IsAlive)
                return;

            listenerThread = new Thread(() => {

                byte[] data;
                IPEndPoint iep=new IPEndPoint(IPAddress.Any, this.listeningPort);
                while (true)
                {
                    try
                    {
                        data = udpListening.Receive(ref iep);
                        string analyzeResult = AnalyzeFunc(data);
                        CallBackFunc(analyzeResult);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });
            listenerThread.IsBackground = true;
            listenerThread.Start();
        }
    }
}
