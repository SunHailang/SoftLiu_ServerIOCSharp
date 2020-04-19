using SoftLiu_ServerIOCSharp.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SoftLiu_ServerIOCSharp.ServerData
{
    public enum ErrorType
    {
        None,
        FaviconIcon,
        FileNotExists,
        LoginDataType,
    }

    public class ErrorData
    {
        private HttpListenerResponse m_response = null;

        private ErrorType m_type = ErrorType.None;

        private string m_errorText = "";

        public ErrorData(HttpListenerResponse response, ErrorType type, string errorText)
        {
            this.m_response = response;
            this.m_type = type;
            this.m_errorText = errorText;
        }

        public void Response()
        {
            switch (m_type)
            {
                case ErrorType.FileNotExists:
                    byte[] bufferFileNotExists = Encoding.UTF8.GetBytes(this.m_errorText);
                    WriteFile(bufferFileNotExists);
                    break;
                case ErrorType.FaviconIcon:
                    string faviconPath = "../../../Resources/favicon.ico";
                    byte[] bufferFaviconIcon = null;
                    FileInfo fileInfoFaviconIcon = null;
                    if (File.Exists(faviconPath))
                    {
                        fileInfoFaviconIcon = new FileInfo(faviconPath);
                    }
                    else
                    {
                        bufferFaviconIcon = Encoding.UTF8.GetBytes(this.m_errorText);
                        //bufferFaviconIcon = Encoding.UTF8.GetBytes("<link rel=\"shortcut icon\" type=\"image/x-icon\" href=\"favicon.ico\">");
                    }
                    WriteFile(bufferFaviconIcon, fileInfoFaviconIcon);
                    break;
                default:
                    byte[] bufferNone = Encoding.UTF8.GetBytes(this.m_errorText);
                    WriteFile(bufferNone);
                    break;
            }
        }

        private void WriteFile(byte[] buffer, FileInfo info = null)
        {
            if (buffer != null)
            {
                m_response.ContentLength64 = buffer.Length;
                m_response.ContentEncoding = Encoding.UTF8;
                m_response.ContentType = StringUtils.GetContneTypeByKey(".json");
                //使用Writer输出http响应代码
                using (BinaryWriter bw = new BinaryWriter(m_response.OutputStream))
                {
                    bw.Write(buffer, 0, buffer.Length);
                    bw.Flush();
                    bw.Close();
                }
                m_response.StatusCode = (int)HttpStatusCode.OK;
                this.m_response.OutputStream.Close();
            }
            else
            {
                //using (StreamReader sr = new StreamReader(File.OpenRead(info.FullName)))
                //{
                //    using (StreamWriter sw = new StreamWriter(m_response.OutputStream))
                //    {
                //        //准备发送到客户端的网页
                //        string responseBody = sr.ReadToEnd();// "<html><head><title>这是一个web服务器的测试</title></head><body><h1>Hello World.</h1></body></html>";
                //        //设置响应头部内容，长度及编码
                //        m_response.ContentLength64 = System.Text.Encoding.UTF8.GetByteCount(responseBody);
                //        m_response.ContentType = StringUtils.GetContneTypeByKey(info.Extension);
                //        m_response.ContentEncoding = Encoding.UTF8;
                //        sw.Write(responseBody);
                //    }
                //    m_response.StatusCode = (int)HttpStatusCode.OK;
                //    this.m_response.OutputStream.Close();
                //}

                using (FileStream fs = File.OpenRead(info.FullName))
                {
                    m_response.ContentLength64 = fs.Length;
                    m_response.ContentEncoding = Encoding.UTF8;
                    m_response.ContentType = StringUtils.GetContneTypeByKey(info.Extension);
                    //m_response.AddHeader("Content-disposition", string.Format("attachment; filename={0};", info.Name));
                    //使用Writer输出http响应代码
                    byte[] fileBuffer = new byte[1024 * 64];
                    int read = 0;
                    using (BinaryWriter bw = new BinaryWriter(m_response.OutputStream))
                    {
                        while ((read = fs.Read(fileBuffer, 0, fileBuffer.Length)) > 0)
                        {
                            bw.Write(fileBuffer, 0, read);
                            bw.Flush();
                        }
                    }
                    m_response.StatusCode = (int)HttpStatusCode.OK;
                    this.m_response.OutputStream.Close();
                }
            }

        }
    }
}
