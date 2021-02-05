using System;
using System.Collections.Generic;
using System.Net.Sockets;
using TFramework.Utils;

namespace SoftLiu_ServerIOCSharp.SocketData
{
    public abstract class ActionData
    {
        private Socket m_client;

        public virtual void Init(Socket client, string recvJson)
        {
            m_client = client;
        }

        protected void SendResponseData(Dictionary<string, object> data, string action, int errcode)
        {
            if (m_client != null && m_client.Connected)
            {
                Dictionary<string, object> responseDic = new Dictionary<string, object>();
                responseDic.Add("action", action);
                responseDic.Add("data", data);
                responseDic.Add("errcode", errcode);
                string responseJson = JsonUtils.Instance.ObjectToJson(responseDic);

                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(responseJson);
                m_client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, (ar) =>
                {
                    Socket socket = ar.AsyncState as Socket;
                // 发送数据长度
                int bytesLen = m_client.EndSend(ar);

                }, m_client);
            }
        }
    }

    public class StateBuffer
    {
        // 接收数据缓冲区 大小： 1M
        private const int bufSize = 1024 * 1024;

        public byte[] buffer = new byte[bufSize];
    }
}
