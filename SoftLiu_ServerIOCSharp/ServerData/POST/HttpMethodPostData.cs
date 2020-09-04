using SoftLiu_ServerIOCSharp.ServerData.POST.Data;
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

        private string m_function = string.Empty;

        private HttpListenerResponse m_response = null;
        public HttpListenerResponse ListenerResponse
        {
            get { return m_response; }
            set { m_response = value; }
        }

        public IFunctionData FunctionData
        {
            get;
            set;
        }

        public HttpMethodPostData(HttpListenerRequest request, HttpListenerResponse response)
        {
            this.m_request = request;
            this.m_response = response;

            Console.WriteLine("POST: " + this.m_request.RawUrl.Trim('/'));
            m_function = this.m_request.RawUrl.Trim('/');
            switch (m_function)
            {
                case "Login":
                    FunctionData = new LoginData(m_request, m_response);
                    break;
                default:
                    break;
            }

        }

        public void Response()
        {
            if (FunctionData != null)
            {
                FunctionData.Response();
            }
            else
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
}
