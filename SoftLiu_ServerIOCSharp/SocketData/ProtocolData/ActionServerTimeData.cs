using System;
using System.Collections.Generic;
using TFramework.Utils;
using System.Net.Sockets;

namespace SoftLiu_ServerIOCSharp.SocketData.ProtocolData
{
    public class ActionServerTimeData : ActionData
    {
        public ActionServerTimeData(Socket client) : base(client)
        {
        }

        public override void Init(string recvJson)
        {
            // {"action":"serverTime"}
            Dictionary<string, object> dataRecvDic = JsonUtils.Instance.JsonToObject<Dictionary<string, object>>(recvJson);
            if (dataRecvDic != null && dataRecvDic.ContainsKey("action"))
            {
                DateTime now = DateTime.Now;
                string timeStamp = TimeUtils.GetTimeStamp(now);
                string time = now.ToString("yyy-MM-dd HH:mm:ss");
                int errcode = 0;
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("time", time);
                data.Add("timestamp", timeStamp);
                SendResponseData(data, dataRecvDic["action"].ToString(), errcode);
            }
        }
    }
}
