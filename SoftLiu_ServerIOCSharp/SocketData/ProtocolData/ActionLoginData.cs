using System;
using System.Collections.Generic;
using System.Net.Sockets;
using TFramework.Utils;

namespace SoftLiu_ServerIOCSharp.SocketData.ProtocolData
{
    public class ActionLoginData : ActionData
    {
        public override void Init(Socket client, string recvJson)
        {
            base.Init(client, recvJson);
            //Console.WriteLine($"ActionLoginData Response: {responseJson}");
            Dictionary<string, object> dataRecvDic = JsonUtils.Instance.JsonToObject<Dictionary<string, object>>(recvJson);
        }
    }
}
