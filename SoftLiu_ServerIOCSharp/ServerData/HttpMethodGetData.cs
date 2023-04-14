using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SoftLiu_ServerIOCSharp.ServerData
{
    public class HttpMethodGetData : IHttpMethodData
    {
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
        
        public ErrorData ErrorData
        {
            get;
            set;
        }

        private string m_function = string.Empty;

        public HttpMethodGetData(HttpListenerRequest request, HttpListenerResponse response)
        {
            this.m_request = request;
            this.m_response = response;

            //NameValueCollection headers = this.m_request.Headers;
            //CookieCollection cookies = this.m_request.Cookies;
            //if (headers != null)
            //{
            //    string[] values = headers.GetValues("Content-Function");
            //    if (values != null)
            //    {
            //        m_function = values.FirstOrDefault();
            //    }
            //}

            Debug.Log(this.m_request.RawUrl.Trim('/'));
            m_function = this.m_request.RawUrl.Trim('/');
            switch (m_function)
            {
                case "HotFixRes":
                    m_functionData = new HotFixDownload.HotFixDownloadData(request, response);
                    break;
                case "AssetBundles":
                    m_functionData = new AssetBundleDownloadData(request, response);
                    break;
                case "ServerTime":
                    m_functionData = new ServerTimeData(response);
                    break;
                case "PackageUpdate":
                    m_functionData = new PackageUpdateData(request, response);
                    break;
                case "favicon.ico":
                    ErrorData = new ErrorData(this.m_response, ErrorType.FaviconIcon, "HttpMethodGetData favicon.ico Request.");
                    break;
                default:
                    ErrorData = new ErrorData(this.m_response, ErrorType.None, "HttpMethodGetData Unknow Error.");
                    break;
            }
        }
        
        public void Response()
        {
            if (m_functionData != null)
            {
                m_functionData.Response();
            }
            else
            {
                ErrorData.Response();
            }
        }
    }
}
