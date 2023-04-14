using System;
using System.Collections.Generic;
using System.Net.Sockets;
using TFramework.Utils;

namespace SoftLiu_ServerIOCSharp.SocketData.ProtocolData
{
    public class ActionLoginData : ActionData
    {
        public ActionLoginData(Socket client) : base(client)
        {
        }

        public override void Init(string recvJson)
        {
            //Debug.Log($"ActionLoginData Response: {responseJson}");
            Dictionary<string, object> dataRecvDic = JsonUtils.Instance.JsonToObject<Dictionary<string, object>>(recvJson);
        }
    }
}
