using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SoftLiu_ServerIOCSharp.ServerData
{
    public class HttpMethodPostData : IHttpMethodData
    {
        private ErrorData m_errorData = null;
        public ErrorData ErrorData
        {
            get { return m_errorData; }
            set { m_errorData = value; }
        }

        private HttpListenerRequest m_request = null;
        public HttpListenerRequest ListenerRequest
        {
            get { return m_request; }
            set { m_request = value; }
        }

        private HttpListenerResponse m_response = null;
        public HttpListenerResponse ListenerResponse
        {
            get { return m_response; }
            set { m_response = value; }
        }

        private IFunctionData m_functionData = null;
        public IFunctionData FunctionData
        {
            get { return m_functionData; }
            set { m_functionData = value; }
        }

        public HttpMethodPostData(HttpListenerRequest request, HttpListenerResponse response)
        {
            this.m_request = request;
            this.m_response = response;


            using (Stream s = m_request.InputStream)
            {
                using (StreamReader reader = new StreamReader(s, Encoding.UTF8))
                {
                    string content = reader.ReadToEnd();
                    Console.WriteLine(content);
                }
            }

        }

        public void Response()
        {
            //使用Writer输出http响应代码
            using (StreamWriter writer = new StreamWriter(this.m_response.OutputStream))
            {
                writer.Write("收到!");

                m_response.StatusCode = (int)HttpStatusCode.OK;

                writer.Close();
                this.m_response.OutputStream.Close();
            }
        }
    }
}
