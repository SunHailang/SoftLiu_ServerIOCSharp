using SoftLiu_ServerIOCSharp.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SoftLiu_ServerIOCSharp.ServerData
{
    public class AssetBundleDownloadData : IFunctionData
    {
        private ErrorData m_errorData = null;
        public ErrorData ErrorData
        {
            get { return m_errorData; }
            set { m_errorData = value; }
        }
        HttpListenerRequest m_request = null;

        HttpListenerResponse m_response = null;


        private string m_platform = string.Empty;

        private string m_fileName = string.Empty;

        private long m_fileLength = 0;

        public AssetBundleDownloadData(HttpListenerRequest request, HttpListenerResponse response)
        {
            m_request = request;
            m_response = response;

            NameValueCollection headers = this.m_request.Headers;
            CookieCollection cookies = this.m_request.Cookies;
            if (headers != null)
            {
                string[] values = headers.GetValues("Content-Platform");
                if (values != null)
                {
                    m_platform = values.FirstOrDefault();
                }
            }
            if (!string.IsNullOrEmpty(m_platform))
            {
                string m_fileDir = new DirectoryInfo("../../../Resources/GameData/AssetBundles").FullName;
                if (!Directory.Exists(m_fileDir))
                {
                    m_errorData = new ErrorData(this.m_response, ErrorType.None);
                }
                else
                {
                    DirectoryInfo infoDir = new DirectoryInfo(m_fileDir);
                    string strFilePath = infoDir.FullName + "/" + m_platform;
                    m_fileName = infoDir.FullName + "/" + m_platform + ".zip";
                    SharpZipUtility.ZipFie(strFilePath, m_fileName);
                    if (!File.Exists(m_fileName))
                    {
                        m_errorData = new ErrorData(this.m_response, ErrorType.None);
                    }
                }
            }
            else
            {
                m_errorData = new ErrorData(this.m_response, ErrorType.None);
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
                using (FileStream fs = File.OpenRead(m_fileName))
                {
                    m_response.ContentEncoding = Encoding.UTF8;
                    m_response.ContentLength64 = fs.Length;
                    m_response.SendChunked = false;
                    m_response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
                    m_response.AddHeader("Content-disposition", "attachment; filename=" + Path.GetFileName(m_fileName));

                    byte[] buffer = new byte[1024 * 64];
                    int read = 0;
                    using (BinaryWriter bw = new BinaryWriter(m_response.OutputStream))
                    {
                        while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bw.Write(buffer, 0, read);
                            bw.Flush();
                        }
                        //response.ContentLength64 = hello.Length;
                        //bw.Write(hello, 0, hello.Length);
                        bw.Close();
                    }
                }
                m_response.StatusCode = (int)HttpStatusCode.OK;
                m_response.StatusDescription = "OK";
                m_response.OutputStream.Close();
            }
        }

        public void WriteFile(FileInfo source)
        {
            FileInfo item = source;
            // new FileInfo(source.FullName + "/version.txt");
            //foreach (DirectoryInfo dir in source.GetDirectories())
            //{
            //    WriteFile(dir);
            //}
            //foreach (var item in source.GetFiles())


        }
    }
}
