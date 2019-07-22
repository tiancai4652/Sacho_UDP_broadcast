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
        /// 消息分隔符
        /// </summary>
        string MsgSplitter { get; set; }

        /// <summary>
        /// 监听端口
        /// </summary>
        public int listeningPort { get; set; }

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
        public Func<byte[],string,string> AnalyzeFunc { get; set; }

        /// <summary>
        /// 回调方法
        /// </summary>
        public Func<string, string> CallBackFunc { get; set; }

        public BroadcastListener(int listeningPort, string msgSplitter, Func<byte[],string, string> analyzeFunc =null, Func<string, string> callBackFunc=null)
        {
            MsgSplitter = msgSplitter;
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
                IPEndPoint iep = new IPEndPoint(IPAddress.Any, this.listeningPort);
                while (true)
                {
                    try
                    {
                        data = udpListening.Receive(ref iep);
                        if (AnalyzeFunc != null)
                        {
                            string analyzeResult = AnalyzeFunc(data, MsgSplitter);
                            if (CallBackFunc != null)
                            {
                                CallBackFunc(analyzeResult);
                            }
                        }
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
