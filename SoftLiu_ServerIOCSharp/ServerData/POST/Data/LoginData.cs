using SoftLiu_ServerIOCSharp.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TFramework.Utils;

namespace SoftLiu_ServerIOCSharp.ServerData.POST.Data
{
    class LoginData : IFunctionData
    {
        public ErrorData ErrorData
        {
            get;
            set;
        }

        private HttpListenerRequest m_request = null;
        private HttpListenerResponse m_response = null;

        private Dictionary<string, object> m_responseDic;

        public LoginData(HttpListenerRequest request, HttpListenerResponse response)
        {
            this.m_request = request;
            this.m_response = response;
            m_responseDic = new Dictionary<string, object>();
            try
            {
                using (Stream s = m_request.InputStream)
                {
                    using (StreamReader reader = new StreamReader(s, Encoding.UTF8))
                    {
                        string content = reader.ReadToEnd();
                        Debug.Log("InputStream: " + content);
                        Dictionary<string, object> inputDic = JsonUtils.Instance.JsonToObject<Dictionary<string, object>>(content);
                        // 有两个参数 username， password , state
                        string username = inputDic["username"].ToString();
                        string password = inputDic["password"].ToString();
                        int state = (int)inputDic["state"];
                        Debug.Log("InputStream Dictionary Length: " + (inputDic == null ? -1 : inputDic.Count));
                        switch (state)
                        {
                            case -1:
                                // Logout
                                m_responseDic.Add("state", -1);
                                break;
                            case 0:
                                //Login
                                m_responseDic.Add("state", 0);
                                break;
                            case 1:
                                // breath pack
                                m_responseDic.Add("state", 1);
                                break;
                            default:
                                ErrorData = new ErrorData(this.m_response, ErrorType.LoginDataType, "LoginData Error: Unknow Error: ");
                                break;
                        }
                    }
                }
            }
            catch (Exception error)
            {
                // 初始化 一个 ErrorData
                ErrorData = new ErrorData(this.m_response, ErrorType.None, "LoginData Error: Unknow Error: " + error.Message);
            }
        }

        public void Response()
        {
            if (ErrorData != null)
            {
                ErrorData.Response();
            }
            else
            {
                string json = JsonUtils.Instance.ObjectToJson(m_responseDic); 
                byte[] buffer = Encoding.UTF8.GetBytes(json);
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
        }
    }
}
