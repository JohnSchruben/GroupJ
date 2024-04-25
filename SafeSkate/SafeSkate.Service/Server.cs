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
        // are we listening.
        private bool isListening = false;

        // listener.
        private TcpListener tcoListener;

        // list of connected clients.
        private List<ClientNode> clientSockets;

        // active client.
        private TcpClient tcpClient;

        private int port;

        // this is the most important copy of the model. it is the origin and source for all map markers.
        private ObservableCollection<MapMarkerInfo> markers;
        
        // background thread. 
        private Thread thread;
        
        // starts the service thread
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

        // called when a client connection is accepted
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
                // stop accepting
                tclient = tcpl.EndAcceptTcpClient(iar);

                // start accepting again
                tcpl.BeginAcceptTcpClient(OnCompleteAcceptTcpClient, tcpl);

                // lock the client sockets while adding a new one.
                lock (clientSockets)
                {
                    // add newly connected client node in List
                    clientSockets.Add((cNode = new ClientNode(
                        tclient,
                        new byte[ushort.MaxValue],
                        new byte[ushort.MaxValue],
                        tclient.Client.RemoteEndPoint.ToString()
                    )));
                }

                // read
                tclient.GetStream().BeginRead(cNode.Rx, 0, cNode.Rx.Length, OnCompleteReadFromTCPClientStream, tclient);
            }
            catch (Exception exc)
            {
                // handle exception here, usually a client disconnect. 
            }
        }

        // This function reads the data being sent by a client. 
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
                    {
                        nCountReadBytes = tcpClient.GetStream().EndRead(iar);
                    }

                    // Disconnect Client if there is no byte
                    if (nCountReadBytes == 0)
                    {
                        clientSockets.Remove(client);
                        return;
                    }

                    // the bytes received as an xml text string.
                    strRecv = Encoding.ASCII.GetString(client.Rx, 0, nCountReadBytes).Trim();
                    try
                    {
                        // descrializing the xml into an update message
                        var message = SafeSkateSerializer.DeserializeMapMarkerUpdateMessage(strRecv);

                        // we received a legit message 
                        if (message != null)
                        {
                            // add or remove the mesage
                            if (message.IsAdded)
                            {
                                Console.Write("Adding marker:");
                                markers.Add(message.Info);
                            }
                            else
                            {
                                Console.Write("Removing marker:");
                                var marker = markers.FirstOrDefault(x => x.Id == message.Info.Id);
                                markers.Remove(marker);
                            }

                            Console.WriteLine($"total = {markers.Count}");

                            // send the update to every connected client. this is the most crutial part of our setup 
                            BroadcastClients(strRecv);
                        }
                    }
                    catch
                    {
                        // could not serialize message.
                    }

                    // finish reading
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

        // forwards a message that we received from a single client to all of the other currently active clients. basically push notifications. 
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

        // checks if client is connected. also keeps the connection alive.
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