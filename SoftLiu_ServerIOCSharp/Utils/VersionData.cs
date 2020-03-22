using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SoftLiu_ServerIOCSharp.Utils
{
    public class VersionData
    {
        public string m_versionName = "0.1.0";
        public int m_versionCode = 1;

        public VersionData()
        {

        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
