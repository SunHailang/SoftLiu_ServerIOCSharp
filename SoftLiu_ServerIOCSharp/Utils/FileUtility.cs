using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftLiu_ServerIOCSharp.Utils
{
    public static class FileUtility
    {
        /// <summary>
        /// 拷贝一个文件夹内容到另一个文件夹下， 并覆盖
        /// </summary>
        /// <param name="source">源文件夹</param>
        /// <param name="target">目标文件夹</param>
        /// <param name="child">是否包含子文件夹</param>
        /// <param name="withoutExtensions">不包含的 扩展名 ， eg: .meta </param>
        public static void CopyDirectoryFiles(DirectoryInfo source, DirectoryInfo target, bool child = false, bool overwrite = false, params string[] withoutExtensions)
        {
            try
            {
                if (child)
                {
                    foreach (DirectoryInfo dir in source.GetDirectories())
                    {
                        CopyDirectoryFiles(dir, target.CreateSubdirectory(dir.Name), child, overwrite, withoutExtensions);
                    }
                }
                foreach (FileInfo file in source.GetFiles())
                {
                    if (withoutExtensions != null && withoutExtensions.Contains(file.Extension))
                    {
                        continue;
                    }
                    file.CopyTo(Path.Combine(target.FullName, file.Name), overwrite);
                }
            }
            catch (Exception error)
            {
                Debug.LogError("CopyFileRecursively Error: " + error.Message);
            }
        }
        /// <summary>
        /// 获取文件夹内文件的个数
        /// </summary>
        /// <param name="target">目标文件夹</param>
        /// <param name="count">out 个数 默认：0</param>
        /// <param name="child">是否包含子文件夹， 默认：不包含</param>
        public static void GetDirectoryFilesCount(DirectoryInfo target, out long count, bool child = false)
        {
            count = 0;
            try
            {
                if (target == null)
                {
                    return;
                }
                FileInfo[] infos = target.GetFiles();
                if (infos == null)
                {
                    return;
                }
                count += target.GetFiles().Length;
                if (child)
                {
                    DirectoryInfo[] childDirs = target.GetDirectories();
                    if (childDirs == null)
                    {
                        return;
                    }
                    foreach (DirectoryInfo dir in childDirs)
                    {
                        GetDirectoryFilesCount(dir, out count, child);
                    }
                }
            }
            catch (Exception error)
            {
                Debug.LogError("Get Directory Files Count Error: " + error.Message);
            }
        }
    }
}
