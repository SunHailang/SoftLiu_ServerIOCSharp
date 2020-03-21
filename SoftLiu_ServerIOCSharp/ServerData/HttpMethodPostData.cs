using System;
using System.Collections.Generic;
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

        public void Response()
        {
            
        }
    }
}
