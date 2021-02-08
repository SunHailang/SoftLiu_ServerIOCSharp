using System;
using System.Collections.Generic;
using TFramework.Utils;
using System.Net.Sockets;

namespace SoftLiu_ServerIOCSharp.SocketData.ProtocolData
{
    public class ActionPingData : ActionData
    {
        public override void Init(Socket client, string recvJson)
        {
            base.Init(client, recvJson);
            //Console.WriteLine($"ActionMatchconfData Response: {responseJson}");
            Dictionary<string, object> dataRecvDic = JsonUtils.Instance.JsonToObject<Dictionary<string, object>>(recvJson);
            if (dataRecvDic != null && dataRecvDic.ContainsKey("action"))
            {
                if (client != null && client.Connected)
                {
                    string action = dataRecvDic["action"].ToString();
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    SendResponseData(data, action, 0);
                }
            }
        }
    }
}
