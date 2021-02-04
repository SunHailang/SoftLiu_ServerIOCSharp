﻿using SoftLiu_ServerIOCSharp.SocketData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFramework.Singleton;
using TFramework.Utils;

namespace SoftLiu_ServerIOCSharp.Misc
{
    public class MiscManager : AutoGeneratedSingleton<MiscManager>, IDisposable
    {
        public const string pathContentTypeJson = "Misc/ContentType.json";
        private const string m_pathSocket = "Misc/SocketServerData.json";

        private Dictionary<string, string> m_ContentTypeDic = null;
        public Dictionary<string, string> ContentTypeDic { get { return m_ContentTypeDic; } }
        public MiscManager()
        {
            m_ContentTypeDic = new Dictionary<string, string>();
        }

        public void Init()
        {
            using (StreamReader sr = new StreamReader(File.OpenRead(pathContentTypeJson)))
            {
                string read = sr.ReadToEnd();
                m_ContentTypeDic = JsonUtils.Instance.JsonToObject<Dictionary<string, string>>(read);
            }

            using (StreamReader sr = new StreamReader(File.OpenRead(m_pathSocket)))
            {
                string read = sr.ReadToEnd();
                Dictionary<string, List<object>> jsonData = JsonUtils.Instance.JsonToObject<Dictionary<string, List<object>>>(read);
                if (jsonData != null)
                {
                    if (jsonData.ContainsKey("ReceiveProtocolData"))
                    {
                        List<object> protocolDatas = jsonData["ReceiveProtocolData"];
                        List<SocketProtocolData> datas = DataUtils.CreateInstances<SocketProtocolData>(protocolDatas);
                        SocketManager.Instance.SetProtocolDatas(datas);
                    }
                }
            }
        }

        public void Dispose()
        {
            m_ContentTypeDic = null;
        }
    }
}