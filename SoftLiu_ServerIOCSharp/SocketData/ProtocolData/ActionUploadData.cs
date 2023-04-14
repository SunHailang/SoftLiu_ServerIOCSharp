using System;
using System.Net.Sockets;

namespace SoftLiu_ServerIOCSharp.SocketData.ProtocolData
{
    public class ActionUploadData : ActionData
    {
        public ActionUploadData(Socket client) : base(client)
        {
        }

        public override void Init(string recvJson)
        {
            Debug.Log($"ActionUploadData Response: {recvJson}");
        }
    }
}
