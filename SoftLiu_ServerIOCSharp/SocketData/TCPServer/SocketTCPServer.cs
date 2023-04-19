using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TFramework.Utils;
using ProtoBuf;

namespace SoftLiu_ServerIOCSharp.SocketData.TCPServer
{
    public class SocketTCPServer : IDisposable
    {
        private Socket m_tcpSocket = null;

        private Dictionary<string, ProtobufClient> m_tcpSocketClientList = null;

        private byte[] m_recvBuffer = null;

        public SocketTCPServer(string socketIP, string socketPort)
        {
            int port = 0;
            if (!int.TryParse(socketPort, out port))
            {
                Debug.Log($"SocketTCPServer Socket TCP Port Error: {socketPort}");
                return;
            }
            m_tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress address = IPAddress.Parse(socketIP);
            IPEndPoint point = new IPEndPoint(address, port);
            m_tcpSocket.Bind(point);
            m_tcpSocket.Listen(10);

            //m_tcpSocket.

            Debug.Log("SocketTCPServer Socket Create Success.");
            m_tcpSocketClientList = new Dictionary<string, ProtobufClient>();
            // 大小设置为 1M
            m_recvBuffer = new byte[1024 * 1024];

            // 获取当前程序集 
            // m_assembly = Assembly.GetExecutingAssembly();
        }
        /// <summary>
        /// 开始异步接收客户端连接
        /// </summary>
        public void StartAsyncSocket()
        {
            m_tcpSocket.BeginAccept(AcceptCallback, m_tcpSocket);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket server = ar.AsyncState as Socket;
                Socket client = server.EndAccept(ar);
                // 有客户端连接进来
                Debug.Log($"SocketTCPServer Client Connect Success, Client:{client.RemoteEndPoint.ToString()}");
                AddClientList(client);
                StartReceive(client);
            }
            catch (Exception error)
            {
                Debug.Log($"SocketTCPServer AcceptCallback Error: {error.Message}");
            }
            finally
            {
                m_tcpSocket.BeginAccept(AcceptCallback, m_tcpSocket);
            }
        }

        private void StartReceive(Socket client)
        {
            try
            {
                client.BeginReceive(m_recvBuffer, 0, m_recvBuffer.Length, SocketFlags.None, ReceiveCallback, client);
            }
            catch (Exception error)
            {
                Debug.LogError($"SocketTCPServer StartReceive Error: {error.Message}");
                if (client != null)
                {
                    Debug.Log($"SocketTCPServer StartReceive Close: {client.RemoteEndPoint.ToString()}");
                    RemoveClientList(client);
                    client.Close();
                }
            }
        }

        private void ReceiveCallback(IAsyncResult iar)
        {
            try
            {
                Socket client = iar.AsyncState as Socket;
                int len = client.EndReceive(iar);
                if (len > 0)
                {
                    string recvData = Encoding.UTF8.GetString(m_recvBuffer, 0, len);
                    // dealwith recv data
                    ActionHandOut(client, recvData);
                }
                StartReceive(client);
            }
            catch (Exception error)
            {
                Debug.LogWarning($"SocketTCPServer ReceiveCallback Error: {error.Message}");
                if (iar != null && (iar.AsyncState as Socket) != null)
                {
                    Socket client = iar.AsyncState as Socket;
                    Debug.Log($"SocketTCPServer ReceiveCallback Close Client: {client.RemoteEndPoint.ToString()}");
                    RemoveClientList(client);
                    client.Close();
                }
            }
        }

        private void ActionHandOut(Socket client, string recvData)
        {
            
        }

        private void AddClientList(Socket client)
        {
            if (client == null)
            {
                return;
            }
            if (!this.m_tcpSocketClientList.ContainsKey(client.RemoteEndPoint.ToString()))
            {
                //ProtobufClient protoClient = new ProtobufClient
                //this.m_tcpSocketClientList.Add(client.RemoteEndPoint.ToString(), client);
            }
        }

        private void RemoveClientList(Socket client)
        {
            if (client == null)
            {
                return;
            }
            if (this.m_tcpSocketClientList.ContainsKey(client.RemoteEndPoint.ToString()))
            {
                this.m_tcpSocketClientList.Remove(client.RemoteEndPoint.ToString());
            }
        }

        public void Dispose()
        {
            if (m_tcpSocket != null)
            {
                m_tcpSocket.Close();
            }
            m_tcpSocket = null;
            m_tcpSocketClientList = null;
            m_recvBuffer = null;
        }
    }
}
