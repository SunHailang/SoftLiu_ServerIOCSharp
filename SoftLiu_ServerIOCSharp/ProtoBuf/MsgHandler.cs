using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;

namespace ProtoBuf
{
    public abstract class MsgHandler
    {
        public abstract void MsgHandleResponse(uint msgId, IMessage msg);
    }
}
