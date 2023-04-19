using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFramework.Protobuf;

namespace SoftLiu_ServerIOCSharp.ProtoBuf
{
    public class MsgRequest
    {
        protected void SendRequest(uint msgId, IMessage msg)
        {
            SocketManager.Instance.SendProtobufMsg(msgId, msg);
        }
    }
}
