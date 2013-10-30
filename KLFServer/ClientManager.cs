using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace KMPServer
{
    class ClientManager
    {
        private List<ServerClient> m_clientList;
        private Server m_server;

        public ClientManager(int max_size, ref Server parent_server)
        {
            this.m_clientList = new List<ServerClient>(max_size);
            this.m_server = parent_server;
        }

        public bool addClient(TcpClient client)
        {
            ServerClient user = new ServerClient(this.m_server, m_clientList.Count);

            return true;
        }
    }
}
