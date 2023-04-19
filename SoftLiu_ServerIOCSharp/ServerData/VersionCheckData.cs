using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;


namespace SoftLiu_ServerIOCSharp.ServerData
{
    public enum VersionCheckType
    {
        None,
        UpdateType,
        LatestType,
    }

    public class VersionCheckData
    {

        private HttpListenerResponse m_response = null;

        private VersionCheckType m_type = VersionCheckType.None;

        private string m_version = string.Empty;

        public VersionCheckData(HttpListenerResponse response, VersionCheckType type, string version = "")
        {
            this.m_response = response;
            this.m_type = type;
            this.m_version = version;
        }

        public void Response()
        {
            CheckData data = new CheckData(m_type, this.m_version);
            byte[] buffer = Encoding.UTF8.GetBytes(data.ToString());
            m_response.ContentEncoding = Encoding.UTF8;
            m_response.ContentType = "application/json;charset=UTF-8";
            m_response.ContentLength64 = buffer.Length;
            using (BinaryWriter bw = new BinaryWriter(m_response.OutputStream))
            {
                bw.Write(buffer, 0, buffer.Length);
                bw.Flush();
            }
            
            this.m_response.StatusCode = (int)HttpStatusCode.OK;
            this.m_response.OutputStream.Close();
            ////使用Writer输出http响应代码
            //using (StreamWriter writer = new StreamWriter(this.m_response.OutputStream))
            //{
            //    CheckData data = new CheckData(m_type, this.m_version);


            //    writer.Write(data.ToString());

            //    this.m_response.StatusCode = (int)HttpStatusCode.OK;
            //    writer.Close();
            //    this.m_response.OutputStream.Close();
            //}
        }
    }

    public class CheckData
    {
        private VersionCheckType m_typeEnum = VersionCheckType.None;
        public string m_type
        {
            get
            {
                return m_typeEnum.ToString();
            }
        }
        public string m_version = string.Empty;

        public CheckData(VersionCheckType type, string version)
        {
            this.m_typeEnum = type;
            this.m_version = version;
        }

        public override string ToString()
        {
            string json = TFramework.Utils.JsonUtils.Instance.ObjectToJson(this);
            return json;
        }
    }
}
