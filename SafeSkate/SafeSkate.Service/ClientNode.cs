using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SafeSkate.Service
{
    internal class ClientNode
    {
        public TcpClient tclient;
        public byte[] Tx, Rx;
        public string strId;
        public string strName;

        public ClientNode(TcpClient _tclient, byte[] _tx, byte[] _rx, string _str, string _name)
        {
            tclient = _tclient;
            Tx = _tx;
            Rx = _rx;
            strId = _str;
            strName = _name;
        }

        public string ToStrng()
        {
            return strName;
        }
    }
}
