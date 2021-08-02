using System;
using System.Net.Sockets;

namespace SoftLiu_ServerIOCSharp.SocketData.ProtocolData
{
    public class ActionPvpVariationData : ActionData
    {
        public ActionPvpVariationData(Socket client) : base(client)
        {
        }

        public override void Init(string recvJson)
        {
            Console.WriteLine($"ActionPvpVariationData Response: {recvJson}");

        }
    }
}
