using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProtoBuf
{
    public class ProtobufClient
    {
        private Socket m_client = null;
        public Socket Client => m_client;

        private ulong m_userId = 0;
        public ulong UserId => m_userId;

        public ProtobufClient(Socket _client, ulong _userId)
        {
            m_client = _client;
            m_userId = _userId;
        }


    }
}
