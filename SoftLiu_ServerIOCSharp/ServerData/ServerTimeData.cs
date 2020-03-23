using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SoftLiu_ServerIOCSharp.Utils;
using System.IO;

namespace SoftLiu_ServerIOCSharp.ServerData
{
    public class ServerTimeData : IFunctionData
    {
        private ErrorData m_errorData = null;
        public ErrorData ErrorData
        {
            get;
            set;
        }


        HttpListenerResponse m_response = null;

        public ServerTimeData(HttpListenerResponse response)
        {
            m_response = response;
        }

        public void Response()
        {
            if (m_errorData != null)
            {
                m_errorData.Response();
            }
            else
            {
                TimeData data = new TimeData();
                //使用Writer输出http响应代码
                using (StreamWriter writer = new StreamWriter(this.m_response.OutputStream))
                {
                    writer.Write(data.ToString());

                    m_response.StatusCode = (int)HttpStatusCode.OK;

                    writer.Close();
                    this.m_response.OutputStream.Close();
                }
            }
        }
    }

    public class TimeData
    {
        public long m_timestamp = 0;
        public string m_times = string.Empty;

        public TimeData()
        {
            m_timestamp = TimeUtility.ConvertDateTimeLong(DateTime.Now);
            m_times = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
