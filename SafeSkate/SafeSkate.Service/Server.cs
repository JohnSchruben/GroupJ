using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SafeSkate.Service
{
    /// <summary>
    ///   Ticket #6 The Service
    /// </summary>
    internal class Server
    {
        private bool isListening = false;
        private TcpListener tcoListener;
        private List<ClientNode> clientSockets;
        private TcpClient tcpClient;
        private int port;
        private ObservableCollection<MapMarkerInfo> markers;
        private Thread thread;

        public void StartServer(int port, ObservableCollection<MapMarkerInfo> markers)
        {
            this.port = port;
            this.markers = markers;
            thread = new Thread(new ThreadStart(this.StartProcessing));
            thread.Start();
        }

        public void StartProcessing()
        {
            clientSockets = new List<ClientNode>();

            try
            {
                tcoListener = new TcpListener(IPAddress.Any, port);
                tcoListener.Start();
                isListening = true;
                tcoListener.BeginAcceptTcpClient(OnCompleteAcceptTcpClient, tcoListener);
            }
            catch (Exception exx)
            {
                // Handle exception message hare
            }
        }

        private void OnCompleteAcceptTcpClient(IAsyncResult iar)
        {
            TcpListener tcpl = (TcpListener)iar.AsyncState;
            TcpClient tclient = null;
            ClientNode cNode = null;
            if (!isListening)
            {
                return;
            }

            try
            {
                tclient = tcpl.EndAcceptTcpClient(iar);

                StreamReader sR = new StreamReader(tclient.GetStream());

                tcpl.BeginAcceptTcpClient(OnCompleteAcceptTcpClient, tcpl);

                lock (clientSockets)
                {
                    // add newly connected client node in List
                    clientSockets.Add((cNode = new ClientNode(
                        tclient,
                        new byte[4096],
                        new byte[4096],
                        tclient.Client.RemoteEndPoint.ToString()
                    )));
                }

                tclient.GetStream().BeginRead(cNode.Rx, 0, cNode.Rx.Length, OnCompleteReadFromTCPClientStream, tclient);
            }
            catch (Exception exc)
            {
                // handle exception here
            }
        }

        private async void OnCompleteReadFromTCPClientStream(IAsyncResult iar)
        {
            int nCountReadBytes = 0;
            string strRecv;
            ClientNode client = null;

            try
            {
                lock (clientSockets)
                {
                    tcpClient = (TcpClient)iar.AsyncState;

                    client = clientSockets.Find(x => x.Address == tcpClient.Client.RemoteEndPoint.ToString());

                    if (IsConnected)
                        nCountReadBytes = tcpClient.GetStream().EndRead(iar);

                    // Disconnect Client if there is no byte
                    if (nCountReadBytes == 0)
                    {
                        clientSockets.Remove(client);
                        return;
                    }

                    strRecv = Encoding.ASCII.GetString(client.Rx, 0, nCountReadBytes).Trim();
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
                    catch
                    {
                        // could not serialize message.
                    }

                    client.Rx = new byte[4096];
                    tcpClient.GetStream().BeginRead(client.Rx, 0, client.Rx.Length, OnCompleteReadFromTCPClientStream, tcpClient);
                }
            }
            catch (Exception ex)
            {
                lock (clientSockets)
                {
                    //Client is Disconnected and removed from list
                    clientSockets.Remove(client);
                }
            }
        }

        public void StopServer()
        {
            StopListing();
        }

        public void StopListing()
        {
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

        private void BroadcastClients(string BroadcastingMsg)
        {
            ClientNode cn = null;

            clientSockets.ForEach(async delegate (ClientNode clntN)
            {
                cn = clntN;

                try
                {
                    var writer = new StreamWriter(cn.TcpClient.GetStream(), Encoding.UTF8) { AutoFlush = true };
                    await writer.WriteAsync(BroadcastingMsg);
                }
                catch (Exception e)
                {
                    // handle exception
                }
            });
        }

        public bool IsConnected
        {
            get
            {
                try
                {
                    if (tcpClient != null && tcpClient.Client != null && tcpClient.Client.Connected)
                    {
                        if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
                        {
                            byte[] buff = new byte[1];
                            return !(tcpClient.Client.Receive(buff, SocketFlags.Peek) == 0);
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