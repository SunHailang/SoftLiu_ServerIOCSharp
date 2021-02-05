using System;
using System.Net.Sockets;

namespace SoftLiu_ServerIOCSharp.SocketData.ProtocolData
{
    public class ActionPvpVariationData : ActionData
    {
        public override void Init(Socket client, string recvJson)
        {
            Console.WriteLine($"ActionPvpVariationData Response: {recvJson}");

        }
    }
}
