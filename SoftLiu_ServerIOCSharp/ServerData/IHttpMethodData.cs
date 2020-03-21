using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SoftLiu_ServerIOCSharp.ServerData
{
    public interface IHttpMethodData
    {
        HttpListenerRequest ListenerRequest { get; set; }

        HttpListenerResponse ListenerResponse { get; set; }

        IFunctionData FunctionData { get; set; }

        ErrorData ErrorData { get; set; }

        void Response();
    }
}
