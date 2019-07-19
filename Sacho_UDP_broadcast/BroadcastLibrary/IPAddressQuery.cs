using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace BroadcastLibrary
{
    public class IPAddressQuery
    {
        /// <summary>
        /// 获取本机IP地址
        /// </summary>
        /// <returns>本机IP地址</returns>
        public static bool GetLocalIP(out IPAddress IPAddress)
        {
            IPAddress = IPAddress.None;
            try
            {
                string HostName = Dns.GetHostName(); 
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        IPAddress = IpEntry.AddressList[i];
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
        static bool GetBroadcastAddressWithPara(IPAddress address, IPAddress subnetMask, out IPAddress BroadcastAddress)
        {
            BroadcastAddress = IPAddress.None;
            try
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
                BroadcastAddress = new IPAddress(broadcastAddress);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取子网掩码
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        static bool GetSubnetMask(IPAddress address, out IPAddress subnetMask)
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
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取广播地址
        /// </summary>
        /// <returns></returns>
        public static bool GetBroadcastAddress(out IPAddress thisBroadcastIP)
        {
            thisBroadcastIP = IPAddress.None;
            try
            {
                IPAddress thisIP = IPAddress.None;
                IPAddress thisSubnetMask = IPAddress.None;

                if (GetLocalIP(out thisIP))
                {
                    if (GetSubnetMask(thisIP, out thisSubnetMask))
                    {
                        if (GetBroadcastAddressWithPara(thisIP, thisSubnetMask, out thisBroadcastIP))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
