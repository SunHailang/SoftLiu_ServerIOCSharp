using SoftLiu_ServerIOCSharp.Algorithm;
using SoftLiu_ServerIOCSharp.Misc;
using SoftLiu_ServerIOCSharp.ServerData;
using SoftLiu_ServerIOCSharp.SocketData.TCPServer;
using SoftLiu_ServerIOCSharp.SocketData.UDPServer;
using SoftLiu_ServerIOCSharp.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TFramework.Utils;

namespace SoftLiu_ServerIOCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            MiscManager.Instance.Init();

            int maxThreadNum, portThreadNum;
            //线程池
            int minThreadNum;
            ThreadPool.SetMaxThreads(10, 10);
            ThreadPool.SetMinThreads(2, 2);

            ThreadPool.GetMaxThreads(out maxThreadNum, out portThreadNum);
            ThreadPool.GetMinThreads(out minThreadNum, out portThreadNum);

            Debug.Log(maxThreadNum + "  -  " + minThreadNum);

            // 启动HTTP服务器
            HttpServer();
            // 启动 TCP Socket服务器
            //SocketTCPServerStart();

            // 启动 UDP Socket服务器
            //SocketUDPServerStart();

            Console.Read();
        }

        #region Http Server
        private static HttpListener listener = null;
        private static void HttpServer()
        {
            string httpServerUrl = ConfigurationUtils.Instance.GetAppSettingValue("HttpServerUrl", "");
            //1：将传入的数据不断放入BlockingCollection，然后使用Task.Factory.StartNew来处理这个队列，也就是所有数据使用一个线程处理
            //2：直接使用ThreadPool.QueueUserWorkItem来处理每条数据,这种方法的处理速度更快，但是因为使用的是多个线程，有时候执行的顺序并不是传入的顺序
            listener = new HttpListener();
            listener.Prefixes.Add(httpServerUrl); //添加需要监听的url范围
            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            listener.Start(); //开始监听端口，接收客户端请求
            Debug.Log("WebServer Starting ...");
            // HttpListener 异步监听
            listener.BeginGetContext(HttpListenerGetContext, listener);
        }
        private static void HttpListenerGetContext(IAsyncResult iar)
        {
            HttpListener lis = iar.AsyncState as HttpListener;
            HttpListenerContext context = lis.EndGetContext(iar);
            ThreadPool.QueueUserWorkItem((ctx) =>
            {
                try
                {
                    ListenerContextData contextData = new ListenerContextData((HttpListenerContext)ctx);
                    contextData.Response();
                }
                catch (Exception e)
                {
                    Debug.LogError("GetContext QueueUserWorkItem Error: " + e.Message);
                }
            }, context);

            listener.BeginGetContext(HttpListenerGetContext, listener);
        }
        #endregion
        private static void SocketTCPServerStart()
        {
            string tcpIP = ConfigurationUtils.Instance.GetAppSettingValue("SocketServerIP", "");
            string tcpPort = ConfigurationUtils.Instance.GetAppSettingValue("SocketTcpServerPort", "");
            SocketTCPServer socketServer = new SocketTCPServer(tcpIP, tcpPort);
            socketServer.StartAsyncSocket();
        }

        private static void SocketUDPServerStart()
        {
            string updIP = ConfigurationUtils.Instance.GetAppSettingValue("SocketServerIP", "");
            string udpPort = ConfigurationUtils.Instance.GetAppSettingValue("SocketUdpServerPort", "");
            SocketUDPServer socketServer = new SocketUDPServer(updIP, udpPort);
        }

        ~Program()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
