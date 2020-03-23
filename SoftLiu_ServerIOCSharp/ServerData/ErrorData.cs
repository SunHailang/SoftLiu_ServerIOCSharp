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
        FileNotExists,
    }

    public class ErrorData
    {
        private HttpListenerResponse m_response = null;

        private ErrorType m_type = ErrorType.None;

        public ErrorData(HttpListenerResponse response, ErrorType type)
        {
            m_response = response;
            m_type = type;
        }

        public void Response()
        {
            switch (m_type)
            {
                case ErrorType.FileNotExists:
                    WriteFile("File Not Exists.");
                    break;
                default:
                    WriteFile("Is Null.");
                    break;
            }
        }

        private void WriteFile(string msg)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(msg);

            m_response.ContentLength64 = buffer.Length;
            m_response.ContentEncoding = Encoding.UTF8;
            m_response.ContentType = "application/json;charset=UTF-8";
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
    }
}
