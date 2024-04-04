using System.Net.Sockets;

namespace SafeSkate.Service
{
    internal class ClientNode
    {
        public TcpClient TcpClient;
        public byte[] Tx, Rx;
        public string Address;

        public ClientNode(TcpClient tcpClient, byte[] tx, byte[] rx, string address)
        {
            TcpClient = tcpClient;
            Tx = tx;
            Rx = rx;
            Address = address;
        }

        public string ToStrng()
        {
            return Address;
        }
    }
}