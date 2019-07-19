using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Sacho_UDP_broadcast
{
    public class MyBroadcast
    {
        public string IPAdress { get; set; }

        public MyBroadcast()
        {
            IPAddress IPAddress = IPAddress.None;
            if (GetLocalIP(out IPAddress))
            {
                IPAdress = IPAddress.ToString();
            }
        }

        public void sendBroadcast(string msg, int port = 8409)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //IPEndPoint iep = new IPEndPoint(IPAddress.Parse("192.168.41.255"), port);
            IPEndPoint iep = new IPEndPoint(IPAddress.Broadcast, port);
            msg = IPAdress + ":" + msg;
            byte[] data = Encoding.UTF8.GetBytes(msg);
            sock.SetSocketOption(SocketOptionLevel.Socket,
                       SocketOptionName.Broadcast, 1);
            sock.SendTo(data, iep);
            sock.Close();
        }

        public void receiveBroadcast()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //IPEndPoint iep = new IPEndPoint(IPAddress.Any, 8409);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 8409);
            EndPoint ep = (EndPoint)iep;
            socket.Bind(iep);
            while (true)
            {
                byte[] buffer = new byte[4096];
                socket.ReceiveFrom(buffer, ref ep);
                var str = Encoding.UTF8.GetString(buffer).TrimEnd('\0'); ;
                if (!str.Contains(IPAdress))
                {
                    Console.WriteLine(str);
                }
            }
        }


        /// <summary>
        /// 获取本机IP地址
        /// </summary>
        /// <returns>本机IP地址</returns>
        public static bool GetLocalIP(out IPAddress IPAddress)
        {
            IPAddress = IPAddress.None;
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        IPAddress= IpEntry.AddressList[i];
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取广播地址
        /// </summary>
        /// <param name="address"></param>
        /// <param name="subnetMask"></param>
        /// <returns></returns>
        public static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        /// <summary>
        /// 获取子网掩码
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool GetSubnetMask(IPAddress address,out IPAddress subnetMask)
        {
            subnetMask = IPAddress.None;
            try
            {
                foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
                {
                    foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                    {
                        if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            if (address.Equals(unicastIPAddressInformation.Address))
                            {
                                subnetMask = unicastIPAddressInformation.IPv4Mask;
                                return true;
                            }
                        }
                    }
                }
                throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
            }
            catch(Exception ex)
            {
                return false;
            }
        }


    }
}
