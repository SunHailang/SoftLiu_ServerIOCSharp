using System;
using System.Net.Sockets;

namespace SoftLiu_ServerIOCSharp.SocketData
{
    public abstract class ActionData
    {
        public abstract void Init(Socket client, string responseJson);
    }
}
