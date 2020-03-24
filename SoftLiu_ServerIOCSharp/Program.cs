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

        static void ProcessRequest(Socket handler)
        {
            // 构造请求报文

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
                    response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
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

                //Task task = Task.Factory.StartNew(() =>
                //{
                while (true)
                {
                    try
                    {
                        HttpListenerContext context = listener.GetContext();

                        Task.Factory.StartNew((ctx) =>
                        {
                            try
                            {
                                //WriteFile((HttpListenerContext)ctx, @"C:\LargeFile.zip");
                                ListenerContextData contextData = new ListenerContextData((HttpListenerContext)ctx);
                                contextData.Response();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Two Error: " + e.Message);
                            }
                        }, context, TaskCreationOptions.LongRunning);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("One Errpr: " + e.Message);
                        //break;
                    }
                }
                //}, TaskCreationOptions.AttachedToParent);

                //while (true)
                //{
                //    HttpListenerContext context = listener.GetContext();
                //    try
                //    {
                //        WriteFile(context, "");
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine("Write File Error: " + e.Message);
                //    }
                //}
            }
        }

        static void PostTask(HttpListenerContext context)
        {
            
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
