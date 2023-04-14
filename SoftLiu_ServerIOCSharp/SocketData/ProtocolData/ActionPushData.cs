using System;
using System.Collections.Generic;
using TFramework.Utils;
using System.Net.Sockets;

namespace SoftLiu_ServerIOCSharp.SocketData.ProtocolData
{
    public class ActionPushData : ActionData
    {
        public ActionPushData(Socket client) : base(client)
        {
        }

        public override void Init(string recvJson)
        {
            Debug.Log($"ActionPushData Response: {recvJson}");
            Dictionary<string, object> dataRecvDic = JsonUtils.Instance.JsonToObject<Dictionary<string, object>>(recvJson);

        }
    }
}
