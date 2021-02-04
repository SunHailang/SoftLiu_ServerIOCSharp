using System;
using System.Collections.Generic;
using TFramework.Utils;
using System.Net.Sockets;

namespace SoftLiu_ServerIOCSharp.SocketData.ProtocolData
{
    public class ActionMatchconfData : ActionData
    {
        public override void Init(Socket client, string responseJson)
        {
            //Console.WriteLine($"ActionMatchconfData Response: {responseJson}");
            Dictionary<string, object> dataRecvDic = JsonUtils.Instance.JsonToObject<Dictionary<string, object>>(responseJson);
        }
    }
}
