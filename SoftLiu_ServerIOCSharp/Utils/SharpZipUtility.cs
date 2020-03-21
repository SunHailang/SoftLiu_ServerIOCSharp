using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;

namespace SoftLiu_ServerIOCSharp.Utils
{
    public static class SharpZipUtility
    {
        public static void ZipFie(string strFile, string strZip)
        {
            //if (strFile[strFile.Length - 1] != Path.AltDirectorySeparatorChar)
            //{
            //    strFile += Path.AltDirectorySeparatorChar;
            //}
            ZipOutputStream outStream = new ZipOutputStream(File.Create(strZip));
            outStream.SetLevel(6);
            ZipCompress(strFile, outStream, strFile);
            outStream.Finish();
            outStream.Close();
        }

        private static void ZipCompress(string strFile, ZipOutputStream outstream, string staticFile)
        {
            //if (strFile[strFile.Length - 1] != Path.DirectorySeparatorChar)
            //{
            //    strFile += Path.DirectorySeparatorChar;
            //}
            Crc32 crc = new Crc32();
            //获取指定目录下所有文件和子目录文件名称
            string[] filenames = Directory.GetFileSystemEntries(strFile);
            //遍历文件
            foreach (string file in filenames)
            {
                if (Directory.Exists(file))
                {
                    ZipCompress(file, outstream, staticFile);
                }
                //否则，直接压缩文件
                else
                {
                    //打开文件
                    FileStream fs = File.OpenRead(file);
                    //定义缓存区对象
                    byte[] buffer = new byte[fs.Length];
                    //通过字符流，读取文件
                    fs.Read(buffer, 0, buffer.Length);
                    //得到目录下的文件（比如:D:\Debug1\test）,test
                    string tempfile = file.Substring(staticFile.LastIndexOf("\\") + 1);
                    ZipEntry entry = new ZipEntry(tempfile);
                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    outstream.PutNextEntry(entry);
                    //写文件
                    outstream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        public static string UnZipFile(string TargetFile, string fileDir)
        {
            string rootFile = "";
            try
            {
                if (!Directory.Exists(fileDir))
                {
                    Directory.CreateDirectory(fileDir);
                }
                //读取压缩文件（zip文件），准备解压缩
                ZipInputStream inputstream = new ZipInputStream(File.OpenRead(TargetFile.Trim()));
                ZipEntry entry;
                string path = fileDir;
                //解压出来的文件保存路径
                string rootDir = "";
                //根目录下的第一个子文件夹的名称
                while ((entry = inputstream.GetNextEntry()) != null)
                {
                    rootDir = Path.GetDirectoryName(entry.Name);
                    //得到根目录下的第一级子文件夹的名称
                    if (rootDir.IndexOf("\\") >= 0)
                    {
                        rootDir = rootDir.Substring(0, rootDir.IndexOf("\\") + 1);
                    }
                    string dir = Path.GetDirectoryName(entry.Name);
                    //得到根目录下的第一级子文件夹下的子文件夹名称
                    string fileName = Path.GetFileName(entry.Name);
                    //根目录下的文件名称
                    if (dir != "")
                    {
                        //创建根目录下的子文件夹，不限制级别
                        if (!Directory.Exists(fileDir + "\\" + dir))
                        {
                            path = fileDir + "\\" + dir;
                            //在指定的路径创建文件夹
                            Directory.CreateDirectory(path);
                        }
                    }
                    else if (dir == "" && fileName != "")
                    {
                        //根目录下的文件
                        path = fileDir;
                        rootFile = fileName;
                    }
                    else if (dir != "" && fileName != "")
                    {
                        //根目录下的第一级子文件夹下的文件
                        if (dir.IndexOf("\\") > 0)
                        {
                            //指定文件保存路径
                            path = fileDir + "\\" + dir;
                        }
                    }
                    if (dir == rootDir)
                    {
                        //判断是不是需要保存在根目录下的文件
                        path = fileDir + "\\" + rootDir;
                    }

                    //以下为解压zip文件的基本步骤
                    //基本思路：遍历压缩文件里的所有文件，创建一个相同的文件
                    if (fileName != String.Empty)
                    {
                        FileStream fs = File.Create(path + "\\" + fileName);
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = inputstream.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                fs.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        fs.Close();
                    }
                }
                inputstream.Close();
                return rootFile;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
