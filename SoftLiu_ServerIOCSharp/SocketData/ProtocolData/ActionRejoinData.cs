using System;
using System.Net.Sockets;

namespace SoftLiu_ServerIOCSharp.SocketData.ProtocolData
{
    public class ActionRejoinData : ActionData
    {
        public ActionRejoinData(Socket client) : base(client)
        {
        }

        public override void Init(string recvJson)
        {
            Console.WriteLine($"ActionRejoinData Response: {recvJson}");
        }
    }
}
