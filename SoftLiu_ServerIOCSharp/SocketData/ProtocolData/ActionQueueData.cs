using System;
using System.Net.Sockets;

namespace SoftLiu_ServerIOCSharp.SocketData.ProtocolData
{
    public class ActionQueueData : ActionData
    {
        public override void Init(Socket client, string responseJson)
        {
            Console.WriteLine($"ActionQueueData Response: {responseJson}");
        }
    }
}
