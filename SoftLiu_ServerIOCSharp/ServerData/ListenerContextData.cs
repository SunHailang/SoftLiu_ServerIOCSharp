using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SoftLiu_ServerIOCSharp.ServerData
{
    public enum MethodType
    {
        NONE,
        GET,
        POST
    }

    public class ListenerContextData
    {
        private HttpListenerContext m_Context = null;

        private MethodType m_type = MethodType.NONE;

        private IHttpMethodData m_httpMethodData = null;

        private ErrorData m_errorData = null;

        public ListenerContextData(HttpListenerContext context)
        {
            this.m_Context = context;
            string httpMethod = this.m_Context.Request.HttpMethod;
            switch (httpMethod.ToUpper())
            {
                case "GET":
                    m_type = MethodType.GET;
                    Console.WriteLine(this.m_Context.Request.UserHostAddress + " -> Use Get Request.");
                    m_httpMethodData = new HttpMethodGetData(this.m_Context.Request, this.m_Context.Response);
                    break;
                case "POST":
                    m_type = MethodType.POST;
                    Console.WriteLine(this.m_Context.Request.UserHostAddress + " -> Use Post Request.");
                    m_httpMethodData = new HttpMethodPostData(this.m_Context.Request, this.m_Context.Response);
                    break;
                default:
                    m_errorData = new ErrorData(this.m_Context.Response, ErrorType.None);
                    break;
            }
        }

        public void Response()
        {
            if (m_httpMethodData != null)
            {
                m_httpMethodData.Response();
            }
            else
            {
                m_errorData.Response();
            }
        }
    }
}
