using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SoftLiu_ServerIOCSharp.ServerData.HotFixDownload
{
    public class HotFixDownloadData : IFunctionData
    {
        private ErrorData m_errorData = null;
        public ErrorData ErrorData
        {
            get { return m_errorData; }
            set { m_errorData = value; }
        }

        private const string m_resPath = @"Resources\HotFixRes\Res";

        private HttpListenerRequest m_request = null;
        private HttpListenerResponse m_response = null;

        private string m_platform = "";
        private string m_requestFile = "";

        private FileInfo m_fileBuffer = null;

        public HotFixDownloadData(HttpListenerRequest request, HttpListenerResponse response)
        {
            m_request = request;
            m_response = response;

            NameValueCollection headers = this.m_request.Headers;
            CookieCollection cookies = this.m_request.Cookies;

            if (headers != null)
            {
                string[] platforms = headers.GetValues("Content-Platform");
                if (platforms != null)
                {
                    m_platform = platforms.FirstOrDefault();
                }
                string[] requestFiles = headers.GetValues("Content-File");
                if (requestFiles != null)
                {
                    m_requestFile = requestFiles.FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(m_platform) && !string.IsNullOrEmpty(m_requestFile))
                {
                    string filePath = Path.Combine(m_resPath, $"{m_platform}/{m_requestFile}");
                    if (!File.Exists(filePath))
                    {
                        m_errorData = new ErrorData(this.m_response, ErrorType.None, "HotFixDownloadData Error: version file not exists.");
                    }
                    else
                    {
                        m_fileBuffer = new FileInfo(filePath);
                    }
                }
                else
                {
                    m_errorData = new ErrorData(this.m_response, ErrorType.None, "AssetBundleDownloadData Error: Unknow error.");
                }
            }
        }


        public void Response()
        {
            if (m_errorData != null)
            {
                m_errorData.Response();
            }
            else
            {
                using (FileStream fs = File.OpenRead(m_fileBuffer.FullName))
                {
                    m_response.ContentEncoding = Encoding.UTF8;
                    m_response.ContentLength64 = fs.Length;
                    m_response.SendChunked = false;
                    m_response.ContentType = Utils.StringUtils.GetContneTypeByKey(m_fileBuffer.Extension);
                    m_response.AddHeader("Content-disposition", "attachment;filename=" + m_fileBuffer.Name);

                    byte[] buffer = new byte[1024 * 64];
                    int read = 0;
                    using (BinaryWriter bw = new BinaryWriter(m_response.OutputStream))
                    {
                        while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bw.Write(buffer, 0, read);
                            bw.Flush();
                        }
                    }
                }
                m_response.StatusCode = (int)HttpStatusCode.OK;
                m_response.StatusDescription = "OK";
                m_response.OutputStream.Close();
            }
        }
    }
}
