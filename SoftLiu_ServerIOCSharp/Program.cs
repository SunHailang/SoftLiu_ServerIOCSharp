using SoftLiu_ServerIOCSharp.Algorithm;
using SoftLiu_ServerIOCSharp.Misc;
using SoftLiu_ServerIOCSharp.ServerData;
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

        static void OrderByName()
        {
            DirectoryInfo dir = new DirectoryInfo("../../../Resources/GameData/AssetBundles/Android");
            FileInfo[] infos = dir.GetFiles();
            FileInfo[] list = infos.OrderBy(file => { return file.Name; }).ToArray();
            foreach (var item in list)
            {
                Console.WriteLine(item.Name);
            }
        }

        static void Main(string[] args)
        {
            MiscManager.Instance.Init();
            AlgorithmManager.Instance.Init();

            //Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //Console.WriteLine(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            //OrderByName();
            //CheckData data = new CheckData(VersionCheckType.UpdateType, "0.1.2");
            //Console.WriteLine(data.ToString());
            //m_envPath = Environment.CurrentDirectory + @"\..\..\cn_windows_10.iso";

            //string path = new DirectoryInfo("../../../../").FullName;
            //Console.WriteLine(path);

            //if (File.Exists(path + "/cn_windows_10.iso"))
            //{
            //    Console.WriteLine("True");
            //}
            //else
            //{
            //    Console.WriteLine("False");
            //}

            //Console.WriteLine(m_envPath);

            //string data = "attachment; filename=" + "cn_windows_10.iso";
            //string[] data1 = data.Split(';');
            //string data2 = "";
            //for (int i = 0; i < data1.Length; i++)
            //{
            //    if (data1[i].Trim().Contains("filename="))
            //    {
            //        data2 = data1[i];
            //        break;
            //    }
            //}
            //string[] data3 = data.Split('=');
            //if (data3.Length==2)
            //{
            //    Console.WriteLine(data3[1]);
            //}
            //Console.WriteLine(data3.Length);

            //string m_fileDir = new DirectoryInfo("../../../Resources/GameData/AssetBundles/" + "Android").FullName;
            //string tar = new DirectoryInfo("../../../").FullName + "Resources/GameData/AssetBundles";
            //SharpZipUtility.ZipFie(m_fileDir, tar + "/android.zip");
            //SharpZipUtility.UnZipFile(tar + "/android.zip", tar);
            //Console.WriteLine("UnZipFile Complete.");

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

        private static void HttpSer()
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

                            //Task.Factory.StartNew((ctx) =>
                            //{
                            //    try
                            //    {
                            //        ListenerContextData contextData = new ListenerContextData((HttpListenerContext)ctx);
                            //        contextData.Response();
                            //    }
                            //    catch (Exception e)
                            //    {
                            //        Console.WriteLine("Two Error: " + e.Message);
                            //    }
                            //}, context, TaskCreationOptions.LongRunning);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("One Errpr: " + e.Message);
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}
