﻿using System;
using System.Net.Sockets;

namespace SoftLiu_ServerIOCSharp.SocketData.ProtocolData
{
    public class ActionRejoinData : ActionData
    {
        public override void Init(Socket client, string recvJson)
        {
            base.Init(client, recvJson);
            Console.WriteLine($"ActionRejoinData Response: {recvJson}");
        }
    }
}
