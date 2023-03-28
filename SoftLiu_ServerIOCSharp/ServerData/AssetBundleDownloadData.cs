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

        private VersionCheckData m_versionCheckData = null;

        private const string m_assetBundlesPath = "Resources/GameData/AssetBundles";
        //private const string m_assetBundlesPath = @"E:\StudyGit\SoftLiu_EmojiText\HotFixRes\"

        private string m_platform = string.Empty;
        private string m_gameID = string.Empty;
        private string m_version = string.Empty;
        private bool m_versionCheck = true;

        private FileInfo m_fileBuffer = null;

        public AssetBundleDownloadData(HttpListenerRequest request, HttpListenerResponse response)
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
                string m_fileDir = new DirectoryInfo(m_assetBundlesPath + "/" + m_gameID + "/" + m_platform).FullName;
                if (!Directory.Exists(m_fileDir))
                {
                    m_errorData = new ErrorData(this.m_response, ErrorType.None, "AssetBundleDownloadData Error: file Directory not exists.");
                }
                else
                {
                    DirectoryInfo infoDir = new DirectoryInfo(m_fileDir);
                    if (m_versionCheck)
                    {
                        FileInfo[] versions = infoDir.GetFiles();
                        if (versions == null || versions.Length <= 0)
                        {
                            m_errorData = new ErrorData(this.m_response, ErrorType.None, "AssetBundleDownloadData Error: not version.");
                            return;
                        }
                        int index = m_version.LastIndexOf('.');
                        string versionName = m_version.Substring(0, index);
                        string versionExt = m_version.Substring(index, m_version.Length - index);
                        FileInfo[] versionArrayData = versions.Where(info => { return (info.Extension == versionExt); }).ToArray();
                        if (versionArrayData == null || versionArrayData.Length <= 0)
                        {
                            m_errorData = new ErrorData(this.m_response, ErrorType.FileNotExists, "AssetBundleDownloadData Error: version arrary data is null.");
                            return;
                        }
                        FileInfo[] versionArray = versions.OrderBy(file => { return file.Name; }).ToArray();
                        FileInfo latestVersionInfo = versionArray[versionArray.Length - 1];
                        string latestVersion = Path.GetFileNameWithoutExtension(latestVersionInfo.FullName);
                        int result = latestVersion.CompareTo(versionName);
                        if (result == 0)
                        {
                            // 最新版
                            m_versionCheckData = new VersionCheckData(m_response, VersionCheckType.LatestType, latestVersionInfo.Name);
                        }
                        else if (result > 0)
                        {
                            // 可更新
                            m_versionCheckData = new VersionCheckData(m_response, VersionCheckType.UpdateType, latestVersionInfo.Name);
                        }
                        else
                        {
                            // 本地大于服务器
                            m_versionCheckData = new VersionCheckData(m_response, VersionCheckType.None, latestVersionInfo.Name);
                        }
                    }
                    else
                    {
                        string fileName = infoDir.FullName + "/" + m_version;
                        if (!File.Exists(fileName))
                        {
                            m_errorData = new ErrorData(this.m_response, ErrorType.None, "AssetBundleDownloadData Error: version file not exists.");
                        }
                        else
                        {
                            m_fileBuffer = new FileInfo(fileName);
                        }
                        //SharpZipUtility.ZipFie(strFilePath, m_fileName);
                    }
                }
            }
            else
            {
                m_errorData = new ErrorData(this.m_response, ErrorType.None, "AssetBundleDownloadData Error: Unknow error.");
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
                    m_response.ContentType = StringUtils.GetContneTypeByKey(m_fileBuffer.Extension);
                    m_response.AddHeader("Content-disposition", "attachment; filename=" + m_fileBuffer.Name);

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
