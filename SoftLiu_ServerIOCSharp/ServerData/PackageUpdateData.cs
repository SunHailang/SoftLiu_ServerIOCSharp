using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SoftLiu_ServerIOCSharp.ServerData
{
    public class PackageUpdateData : IFunctionData
    {
        private const string m_packagePath = "../../../Resources/GameData/PackageData/";

        private ErrorData m_errorData = null;
        public ErrorData ErrorData
        {
            get { return m_errorData; }
            set { m_errorData = value; }
        }

        private HttpListenerRequest m_request = null;
        private HttpListenerResponse m_response = null;

        private string m_gameID = null;
        private string m_platform = null;
        private string m_version = null;
        private bool m_versionCheck = false;

        private FileInfo m_fileBuffer = null;

        private VersionCheckData m_versionCheckData = null;

        public PackageUpdateData(HttpListenerRequest request, HttpListenerResponse response)
        {
            m_request = request;
            m_response = response;

            NameValueCollection headers = this.m_request.Headers;
            CookieCollection cookies = this.m_request.Cookies;
            if (headers != null)
            {
                string[] gameIDs = headers.GetValues("Content-GameID");
                if (gameIDs != null)
                {
                    m_gameID = gameIDs.FirstOrDefault();
                }
                string[] platforms = headers.GetValues("Content-Platform");
                if (platforms != null)
                {
                    m_platform = platforms.FirstOrDefault();
                }
                string[] versions = headers.GetValues("Content-Version");
                if (versions != null)
                {
                    m_version = versions.FirstOrDefault();
                }
                string[] checks = headers.GetValues("Content-VersionCheck");
                if (checks != null)
                {
                    m_versionCheck = Convert.ToBoolean(checks.FirstOrDefault());
                }
            }
            if (!string.IsNullOrEmpty(m_platform) && !string.IsNullOrEmpty(m_gameID) && !string.IsNullOrEmpty(m_version))
            {
                // 包含 包名(com.android.softliu.huawei) - 版本号(0.1.0)
                string[] inputVersions = m_version.Split('-');
                if (inputVersions.Length < 2)
                {
                    m_errorData = new ErrorData(this.m_response, ErrorType.None);
                    return;
                }
                string packName = inputVersions[0];
                string versionName = inputVersions[1];

                string m_fileDir = new DirectoryInfo(m_packagePath + "/" + m_platform + "/" + m_gameID + "/" + packName).FullName;
                if (!Directory.Exists(m_fileDir))
                {
                    m_errorData = new ErrorData(this.m_response, ErrorType.None);
                }
                else
                {
                    DirectoryInfo infoDir = new DirectoryInfo(m_fileDir);
                    FileInfo[] versions = infoDir.GetFiles();
                    if (versions == null || versions.Length <= 0)
                    {
                        m_errorData = new ErrorData(this.m_response, ErrorType.None);
                        return;
                    }
                    FileInfo[] versionArray = versions.OrderBy(file => { return file.Name; }).ToArray();
                    FileInfo latestVersion = versionArray[versionArray.Length - 1];
                    if (m_versionCheck)
                    {
                        string latestVersionName = Path.GetFileNameWithoutExtension(latestVersion.FullName);
                        int result = latestVersionName.CompareTo(versionName);
                        if (result == 0)
                        {
                            // 最新版
                            m_versionCheckData = new VersionCheckData(m_response, VersionCheckType.LatestType, latestVersion.Name);
                        }
                        else if (result > 0)
                        {
                            // 可更新
                            m_versionCheckData = new VersionCheckData(m_response, VersionCheckType.UpdateType, latestVersion.Name);
                        }
                        else
                        {
                            // 本地大于服务器
                            m_versionCheckData = new VersionCheckData(m_response, VersionCheckType.None, latestVersion.Name);
                        }
                    }
                    else
                    {
                        string extensionName = Path.GetExtension(latestVersion.FullName);
                        string fullPath = m_fileDir + "/" + versionName + extensionName;
                        if (File.Exists(fullPath))
                        {
                            m_fileBuffer = new FileInfo(fullPath);
                        }
                        else
                        {
                            m_errorData = new ErrorData(m_response, ErrorType.FileNotExists);
                        }
                    }
                }
            }
            else
            {
                m_errorData = new ErrorData(m_response, ErrorType.None);
            }
        }



        public void Response()
        {
            if (m_errorData != null)
            {
                m_errorData.Response();
            }
            else if (m_versionCheckData != null)
            {
                m_versionCheckData.Response();
            }
            else
            {
                using (FileStream fs = File.OpenRead(m_fileBuffer.FullName))
                {
                    m_response.ContentEncoding = Encoding.UTF8;
                    m_response.ContentLength64 = fs.Length;
                    m_response.SendChunked = false;
                    m_response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
                    m_response.AddHeader("Content-disposition", "attachment; filename=" + Path.GetFileName(m_fileBuffer.FullName));

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
    }
}
