using SoftLiu_ServerIOCSharp.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftLiu_ServerIOCSharp.Utils
{
    public static class StringUtils
    {
        /// <summary>
        /// 根据文件扩展名， 获取对应 Response ContentType
        /// </summary>
        /// <param name="key">文件扩展名</param>
        /// <returns>ContentType</returns>
        public static string GetContneTypeByKey(string key)
        {
            try
            {
                string value = null;
                key = string.Format("*{0}", key);
                if (MiscManager.Instance.ContentTypeDic.TryGetValue(key, out value))
                {
                    return value;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("GetContneTypeByKey Error: " + error.Message);
            }

            return "application/octet-stream";
        }
    }
}
