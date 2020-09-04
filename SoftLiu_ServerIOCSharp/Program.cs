﻿using SoftLiu_ServerIOCSharp.Algorithm;
using SoftLiu_ServerIOCSharp.Misc;
using SoftLiu_ServerIOCSharp.ServerData;
using SoftLiu_ServerIOCSharp.SocketData;
using SoftLiu_ServerIOCSharp.Utils;
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

        private const string m_serverURL =/* "http://192.168.2.111:8080/";//*/"http://localhost:8080/";


        static string m_envPath = string.Empty;


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

            Console.WriteLine(maxThreadNum + "  -  " + minThreadNum);

            // 启动HTTP服务器
            HttpServer();
            // 启动Socket服务器
            SocketTCPServerStart();

            Console.Read();
        }

        private static void WriteFile(HttpListenerContext cxt, string filePath)
        {
            HttpListenerContext m_ctx = cxt;

            //ThreadPool.QueueUserWorkItem(new WaitCallback(TaskProc), ctx);                    

            HttpListenerResponse response = m_ctx.Response;

            string path = new DirectoryInfo("../../../../").FullName;
            string m_filePath = path + "cn_windows_10.iso";


            byte[] hello = Encoding.UTF8.GetBytes("Hello World!");
            //response.SetCookie("Content-Length", hello.Length.ToString());
            //response.ContentType = "text/plain;charset=UTF-8";

            //response.AddHeader("Connection", "Keep-Alive");
            //ctx.Response.OutputStream.Write()

            if (!File.Exists(m_filePath))
            {
                Console.WriteLine("File Not Exists : " + m_filePath);
            }
            else
            {
                response.ContentEncoding = Encoding.UTF8;

                using (FileStream fs = File.OpenRead(m_filePath))
                {
                    response.ContentLength64 = fs.Length;
                    response.SendChunked = false;
                    response.ContentType = StringUtils.GetContneTypeByKey(".iso");
                    response.AddHeader("Content-disposition", "attachment; filename=" + "cn_windows_10.iso");

                    byte[] buffer = new byte[1024 * 64];
                    int read = 0;
                    using (BinaryWriter bw = new BinaryWriter(response.OutputStream))
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
                response.StatusCode = (int)HttpStatusCode.OK;
                response.StatusDescription = "OK";
                response.OutputStream.Close();
            }
            //使用Writer输出http响应代码
            //using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream))
            //{
            //    
            //    writer.Close();
            //    ctx.Response.Close();
            //}

        }

        private static void HttpServer()
        {
            //1：将传入的数据不断放入BlockingCollection，然后使用Task.Factory.StartNew来处理这个队列，也就是所有数据使用一个线程处理
            //2：直接使用ThreadPool.QueueUserWorkItem来处理每条数据,这种方法的处理速度更快，但是因为使用的是多个线程，有时候执行的顺序并不是传入的顺序

            Task task = Task.Factory.StartNew(() =>
            {
                using (HttpListener listener = new HttpListener())
                {
                    listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                    listener.Prefixes.Add(m_serverURL);
                    listener.Start();
                    Console.WriteLine("WebServer Starting ...");

                    while (true)
                    {
                        try
                        {
                            HttpListenerContext context = listener.GetContext();
                            ThreadPool.QueueUserWorkItem((ctx) =>
                            {
                                try
                                {
                                    ListenerContextData contextData = new ListenerContextData((HttpListenerContext)ctx);
                                    contextData.Response();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Two Error: " + e.Message);
                                }
                            }, context);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("One Errpr: " + e.Message);
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        private static void SocketTCPServerStart()
        {
            Task task = Task.Factory.StartNew(() =>
            {
                try
                {
                    SocketTCPServer socketServer = new SocketTCPServer();
                    socketServer.StartAsyncSocket();
                }
                catch (Exception error)
                {
                    Console.WriteLine($"SocketTCPServerStart Error: {error.Message}");
                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}
