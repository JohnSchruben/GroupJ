using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.IO;

namespace SafeSkate.Service
{

    internal class Server
    {
        bool isListening = false;
        TcpListener tcoListener;
        private List<ClientNode> clientSockets;
        TcpClient tcpClient;
        IPAddress ipAddress;
        int port;
        private ObservableCollection<MapMarkerInfo> markers;
        Thread thread;

        public void StartServer(string _IP, int _Port, ObservableCollection<MapMarkerInfo> markers)
        {
            ipAddress = IPAddress.Parse(_IP);
            port = _Port;
            this.markers = markers;
            thread = new Thread(new ThreadStart(this.StartProcessing));
            thread.Start();
        }

        public void StartProcessing()
        {
            //Server is started
            clientSockets = new List<ClientNode>();

            try
            {
                tcoListener = new TcpListener(ipAddress, port);
                //Server is running now
                tcoListener.Start();
                isListening = true;
                tcoListener.BeginAcceptTcpClient(onCompleteAcceptTcpClient, tcoListener);
            }
            catch (Exception exx)
            {
                // Handle exception message hare
            }
        }

        void onCompleteAcceptTcpClient(IAsyncResult iar)
        {
            TcpListener tcpl = (TcpListener)iar.AsyncState;
            TcpClient tclient = null;
            ClientNode cNode = null;
            if (!isListening)
            {
                //Stopped listening for incoming connections
                return;
            }

            try
            {
                tclient = tcpl.EndAcceptTcpClient(iar);
                //Client Connected...
                StreamReader sR = new StreamReader(tclient.GetStream());


                tcpl.BeginAcceptTcpClient(onCompleteAcceptTcpClient, tcpl);

                lock (clientSockets)
                {
                    // add newly connected client node in List
                    clientSockets.Add((cNode = new ClientNode(
                        tclient,
                        new byte[4096],
                        new byte[4096],
                        tclient.Client.RemoteEndPoint.ToString(),
                        "User"        
                    )));
                }

                tclient.GetStream().BeginRead(cNode.Rx, 0, cNode.Rx.Length, onCompleteReadFromTCPClientStream, tclient);
            }
            catch (Exception exc)
            {
                // handle exception here
            }
        }

        async void onCompleteReadFromTCPClientStream(IAsyncResult iar)
        {
            int nCountReadBytes = 0;
            string strRecv;
            ClientNode cn = null;

            try
            {
                lock (clientSockets)
                {
                    tcpClient = (TcpClient)iar.AsyncState;

                    // find client from list
                    cn = clientSockets.Find(x => x.strId == tcpClient.Client.RemoteEndPoint.ToString());
                    //// check if client is connected
                    if (IsConnected)
                        nCountReadBytes = tcpClient.GetStream().EndRead(iar);

                    ////Disconnect Client if there is no byte
                    //if (nCountReadBytes == 0)
                    //{
                    //    clientSockets.Remove(cn);
                    //    return;
                    //}

                    strRecv = Encoding.ASCII.GetString(cn.Rx, 0, nCountReadBytes).Trim();
                    try
                    {

                        var message = SafeSkateSerializer.DeserializeMapMarkerUpdateMessage(strRecv);

                        if (message != null)
                        {
                            if (message.IsAdded)
                            {
                                markers.Add(message.Info);
                            }

                            BroadcastClients(strRecv);
                        }
                    }
                    catch (Exception)
                    {
                        ;
                    }

                    cn.Rx = new byte[4096];

                    tcpClient.GetStream().BeginRead(cn.Rx, 0, cn.Rx.Length, onCompleteReadFromTCPClientStream, tcpClient);


                }
            }
            catch (Exception ex)
            {
                lock (clientSockets)
                {
                    //Client is Disconnected and removed from list
                    clientSockets.Remove(cn);
                }
            }
        }

        private void onCompleteWriteToClientStream(IAsyncResult iar)
        {
            try
            {
                TcpClient tcpc = (TcpClient)iar.AsyncState;
                tcpc.GetStream().EndWrite(iar);
            }
            catch (Exception exc)
            {
                // handle exception
            }
        }

        public void StopServer()
        {
            StopListing();
        }

        public void StopListing()
        {
            // stop server thread
            thread.Interrupt();
            try
            {
                isListening = false;
                tcoListener.Stop();
            }
            catch (Exception eee)
            {
                // handle exception
            }
        }
        void BroadcastClients(string BroadcastingMsg)
        {
            if (clientSockets.Count() <= 0)
                return;
            else
            {
                ClientNode cn = null;

                clientSockets.ForEach(async delegate (ClientNode clntN)
                {
                    cn = clntN;

                    try
                    {
                        // broadcasting online clients list
                        //cn.Tx = Encoding.ASCII.GetBytes(BroadcastingMsg);

                        var writer = new StreamWriter(cn.tclient.GetStream(), Encoding.UTF8) { AutoFlush = true };
                        await writer.WriteAsync(BroadcastingMsg); 
                        //cn.tclient.GetStream().BeginWrite(cn.Tx, 0, cn.Tx.Length, onCompleteWriteToClientStream, cn.tclient);
                    }
                    catch (Exception e)
                    {
                        // handle exception 
                    }
                });
            }
        }
        public bool IsConnected
        {
            get
            {
                try
                {
                    if (tcpClient != null && tcpClient.Client != null && tcpClient.Client.Connected)
                    {
                        // Detect if client disconnected
                        if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
                        {
                            byte[] buff = new byte[1];
                            if (tcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
                            {
                                // Client disconnected
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
