using System;
using System.Collections.Generic;
using TFramework.Utils;
using System.Net.Sockets;

namespace SoftLiu_ServerIOCSharp.SocketData.ProtocolData
{
    public class ActionPushData : ActionData
    {
        public override void Init(Socket client, string recvJson)
        {
            base.Init(client, recvJson);
            Console.WriteLine($"ActionPushData Response: {recvJson}");
            Dictionary<string, object> dataRecvDic = JsonUtils.Instance.JsonToObject<Dictionary<string, object>>(recvJson);

        }
    }
}
