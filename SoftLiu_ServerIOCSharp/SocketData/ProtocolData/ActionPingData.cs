using System;
using System.Collections.Generic;
using TFramework.Utils;
using System.Net.Sockets;

namespace SoftLiu_ServerIOCSharp.SocketData.ProtocolData
{
    public class ActionPingData : ActionData
    {
        public ActionPingData(Socket client) : base(client)
        {
        }

        public override void Init(string recvJson)
        {
            //Debug.Log($"ActionMatchconfData Response: {responseJson}");
            Dictionary<string, object> dataRecvDic = JsonUtils.Instance.JsonToObject<Dictionary<string, object>>(recvJson);
            if (dataRecvDic != null && dataRecvDic.ContainsKey("action"))
            {
                string action = dataRecvDic["action"].ToString();
                Dictionary<string, object> data = new Dictionary<string, object>();
                SendResponseData(data, action, 0);
            }
        }
    }
}
