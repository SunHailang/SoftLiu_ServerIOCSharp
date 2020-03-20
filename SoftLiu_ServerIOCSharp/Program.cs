using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoftLiu_ServerIOCSharp
{
    class Program
    {
        const string m_serverIP = "";
        const int m_serverPort = 20000;

        static bool m_isRunning = false;
        static void Main(string[] args)
        {
            HttpSer();

            // 创建 Socket 服务器
            //Socket serverSocet = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //serverSocet.Bind(new IPEndPoint(IPAddress.Parse(m_serverIP), m_serverPort));
            //serverSocet.Listen(10);
            //m_isRunning = true;

            //while (m_isRunning)
            //{
            //    Socket clientSocket = serverSocet.Accept();
            //    Thread requestThread = new Thread(() => { });
            //    requestThread.Start();
            //}
        }

        static void ProcessRequest(Socket handler)
        {
            // 构造请求报文

        }

        private static void HttpSer()
        {
            using (HttpListener listener = new HttpListener())
            {
                listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                listener.Prefixes.Add("http://localhost:8080/AssetBundles/");
                listener.Start();
                Console.WriteLine("WebServer Starting ...");

                int maxThreadNum, portThreadNum;

                //线程池
                int minThreadNum;
                ThreadPool.SetMaxThreads(10, 10);
                ThreadPool.SetMinThreads(2, 2);

                ThreadPool.GetMaxThreads(out maxThreadNum, out portThreadNum);
                ThreadPool.GetMinThreads(out minThreadNum, out portThreadNum);

                Console.WriteLine(maxThreadNum + "  -  " + minThreadNum);

                while (true)
                {
                    HttpListenerContext ctx = listener.GetContext();

                    //ThreadPool.QueueUserWorkItem(new WaitCallback(TaskProc), ctx);

                    ctx.Response.StatusCode = 200;

                    if (ctx.Request.HttpMethod == "GET")
                    {
                        //string name = ctx.Request.QueryString["name"];
                        //if (!string.IsNullOrEmpty(name))
                        //{
                        //    Console.WriteLine("Request Name: " + name);
                        //}
                        Console.WriteLine(ctx.Request.UserHostAddress + " -> Use Get Request.");
                    }
                    else if (ctx.Request.HttpMethod == "POST")
                    {
                        Console.WriteLine(ctx.Request.UserHostAddress + " -> Use Post Request.");
                    }


                    //使用Writer输出http响应代码
                    using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream))
                    {
                        writer.WriteLine("Hello World!");
                        writer.Close();
                        ctx.Response.Close();
                    }
                }
                //listener.Stop();
            }

        }

        static void PostTask(HttpListenerContext context)
        {
            HttpListenerPostParaHelper httppost = new HttpListenerPostParaHelper(context);
            List<HttpListenerPostValue> lis = httppost.GetHttpListenerPostValue();
        }

        static void TaskProc(object o)
        {
            HttpListenerContext ctx = (HttpListenerContext)o;

            ctx.Response.StatusCode = 200;//设置返回给客服端http状态代码

            //string type = ctx.Request.QueryString["type"];
            //string userId = ctx.Request.QueryString["userId"];
            //string password = ctx.Request.QueryString["password"];
            //string filename = Path.GetFileName(ctx.Request.RawUrl);
            //string userName = HttpUtility.ParseQueryString(filename).Get("userName");//避免中文乱码

            //进行处理

            //使用Writer输出http响应代码
            using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream))
            {
                writer.Write("处理结果");
                writer.Close();
                ctx.Response.Close();
            }
        }
    }
}
