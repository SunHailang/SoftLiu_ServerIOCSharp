using System;
using System.Collections.Generic;
using TFramework.Utils;
using System.Net.Sockets;

namespace SoftLiu_ServerIOCSharp.SocketData.ProtocolData
{
    public class ActionServerTimeData : ActionData
    {
        public override void Init(Socket client, string recvJson)
        {
            //Console.WriteLine($"ActionConnData Response: {responseJson}");
            // {"action":"serverTime"}
            Dictionary<string, object> dataRecvDic = JsonUtils.Instance.JsonToObject<Dictionary<string, object>>(recvJson);
            if (dataRecvDic != null && dataRecvDic.ContainsKey("action"))
            {
                if(client!=null && client.Connected)
                {
                    DateTime now = DateTime.Now;
                    string timeStamp = TimeUtils.GetTimeStamp(now);
                    string time = now.ToString("yyy-MM-dd HH:mm:ss");
                    int errcode = 0;
                    Dictionary<string, object> responseDic = new Dictionary<string, object>();
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    data.Add("time", time);
                    data.Add("timestamp", timeStamp);
                    responseDic.Add("data", data);
                    responseDic.Add("errcode", errcode);
                    string responseJson = JsonUtils.Instance.ObjectToJson(responseDic);
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(responseJson);
                    client.BeginSend(bytes, SocketFlags.None, (ar) =>
                    {

                    }, );
                }
            }
        }
    }
}
