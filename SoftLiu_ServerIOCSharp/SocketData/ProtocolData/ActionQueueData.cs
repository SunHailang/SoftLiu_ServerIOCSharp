using System;
using System.Net.Sockets;

namespace SoftLiu_ServerIOCSharp.SocketData.ProtocolData
{
    public class ActionQueueData : ActionData
    {
        public ActionQueueData(Socket client) : base(client)
        {
        }

        public override void Init(string recvJson)
        {
            Debug.Log($"ActionQueueData Response: {recvJson}");
        }
    }
}
