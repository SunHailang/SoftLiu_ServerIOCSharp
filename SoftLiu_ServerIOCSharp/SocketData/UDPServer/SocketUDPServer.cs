using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TFramework.Utils;

namespace SoftLiu_ServerIOCSharp.SocketData.UDPServer
{
    public class SocketUDPServer
    {
        private Socket m_udpServer = null;

        private byte[] m_recvBuffer = null;

        private EndPoint m_serverEndPoint = null;

        private EndPoint m_clientEndPoint = null;

        private List<EndPoint> m_clientList = new List<EndPoint>();

        public SocketUDPServer(string udpIP, string udpPort)
        {
            Debug.Log("udp server starting...");

            m_recvBuffer = new byte[1024 * 1024];

            m_udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress address = IPAddress.Parse(udpIP);
            int port = 0;
            if (!int.TryParse(udpPort, out port))
            {
                Debug.LogWarning($"SocketTCPServer Socket TCP Port Error: {udpPort}");
                return;
            }
            m_serverEndPoint = new IPEndPoint(address, port);
            m_udpServer.Bind(m_serverEndPoint);

            m_clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

            m_udpServer.BeginReceiveFrom(m_recvBuffer, 0, m_recvBuffer.Length, SocketFlags.None, ref m_clientEndPoint, AcceptCallback, m_udpServer);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            int len = 0;
            try
            {
                len = m_udpServer.EndReceiveFrom(ar, ref m_clientEndPoint);
                if (len > 0)
                {
                    if (!m_clientList.Contains(m_clientEndPoint))
                    {
                        m_clientList.Add(m_clientEndPoint);
                    }

                    string recv = Encoding.UTF8.GetString(m_recvBuffer, 0, len);
                    Debug.Log($"recv callback:{recv} , ip:{m_clientEndPoint.ToString()}");

                    byte[] buffer = Encoding.UTF8.GetBytes("recv");
                    m_udpServer.SendTo(buffer, m_clientEndPoint);
                }
            }
            catch (Exception error)
            {
                Debug.LogError($"SocketTCPServer AcceptCallback Error: {error.Message}");
            }
            finally
            {
                if (m_udpServer != null)
                    m_udpServer.BeginReceiveFrom(m_recvBuffer, 0, m_recvBuffer.Length, SocketFlags.None,
                ref m_clientEndPoint, AcceptCallback, m_udpServer);
            }
        }
    }
}
