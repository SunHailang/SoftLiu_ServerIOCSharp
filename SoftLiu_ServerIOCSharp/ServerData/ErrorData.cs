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
                default:
                    WriteFile("Is Null.");
                    break;
            }
        }

        private void WriteFile(string mes)
        {
            //使用Writer输出http响应代码
            using (StreamWriter writer = new StreamWriter(this.m_response.OutputStream))
            {
                writer.Write(mes);
                
                m_response.StatusCode = (int)HttpStatusCode.OK;               

                writer.Close();
                this.m_response.OutputStream.Close();
            }
        }
    }
}
