using System.Text;
using System.Threading;
using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;


namespace TCPServer {
    public class Server {
        // clients stored in dict
        // careful cause we gonna have multiple threads writing to the dic
        // speareate and wait for other threads to finish
        private static object _lock = new object(); // handling multiple threads writing at the same time 
        private readonly TcpListener server;
        private int clientCount = 1; // id of the client 
        private static Dictionary<int, TcpClient> clients = new Dictionary<int, TcpClient>();
        public Server(string ip, int port) {
            IPAddress localAddress = IPAddress.Parse(ip);
            // creating new server 
            server = new TcpListener(localAddress, port);
            // so far we started but do not listen for new stuff coming     
            server.Start();
            // listening for the connectiions 
            StartListener();
        }

        public void StartListener() {
            try {
                while(true) {
                    // constantly listening for the connections
                    Console.WriteLine("waiting for the client...");
                    TcpClient client = server.AcceptTcpClient();
                    // lock to aviod writing at thesame time to the dic
                    lock(_lock) {
                        clients.Add(clientCount, client);
                        Console.WriteLine("added a new client");
                    }
                    Thread thread = new Thread(HandleClientconnection);
                    thread.Start(clientCount);
                    clientCount++;
                }
            }
            catch(SocketException e) {
                Console.WriteLine($"SSocket exception {e}"); //catching the error
                server.Stop();
            }
        }

        public void HandleClientconnection(Object obj) {
            int clientId = (int)obj; // casting obj to the int
            TcpClient client = null;

            lock(_lock) {
                client = clients[clientId];
            }
            while(true) {
                //creating net stream, gettubg the info from the client 
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int byteCount = stream.Read(buffer, 0, buffer.Length);
                if(byteCount == 0) {
                    break;
                }
                string data = Encoding.ASCII.GetString(buffer, 0, byteCount); // data to ascii
                // client sending the message and all other clients get the message what the mesage is 
                //send data to all clients 
                Broadcast(data); // getting data and sending it to all the clients 
                Console.WriteLine($"client {clientId} is broadcasting {data}"); 
            }
            lock(_lock) {
                // safely disconnectiing the clinet if we do not get any more mesages from the client 
                // removing client form the dic
                clients.Remove(clientId);
            }
            // shutting down the clients
            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();

        } 
        public static void Broadcast(string message) { 
            // writing stream bytes from string

            byte[] buffer = Encoding.ASCII.GetBytes(message+Environment.NewLine);
            // again lock just in case 
            lock(_lock) {
                foreach(TcpClient client in clients.Values) {
                    // loop over all the tcpclients broadcast message and convert it to bytes
                    // as we get a client we write it to the buffer
                    NetworkStream stream = client.GetStream();
                    stream.Write(buffer, 0, buffer.Length);
                }
            }

        }

        

    }
}


// namespace TCPServer
// {
//     public class Server
//     {
//         private static object _lock = new object();

//         private readonly TcpListener server;

//         private int clientCount = 1;

//         private static Dictionary<int, TcpClient> clients = new Dictionary<int, TcpClient>();

//         public Server(string ip, int port)
//         {
//             IPAddress localAddress = IPAddress.Parse(ip);
//             server = new TcpListener(localAddress, port);
//             server.Start();
//             StartListener();
//         }

//         public void StartListener()
//         {
//             try
//             {
//                 while(true)
//                 {  
//                     Console.WriteLine("Waiting for client connections...");
//                     TcpClient client = server.AcceptTcpClient();

//                     lock(_lock)
//                     {
//                         clients.Add(clientCount, client);
//                     }

//                     Console.WriteLine("ClientId {clientCount} is connected and added to our clients...");
//                     Thread thread = new Thread(HandleClientConnection);
//                     thread.Start(clientCount);

//                     clientCount++;
//                 }
//             }
//             catch(SocketException e)
//             {
//                 Console.WriteLine($"SSocketException: {e}");
//                 server.Stop();
//             }
//         }

//         public void HandleClientConnection(Object obj)
//         {
//             int clientId = (int)obj;

//             TcpClient client = null;

//             lock(_lock)
//             {
//                 client = clients[clientId];
//             }

//             while(true)
//             {
//                 NetworkStream stream = client.GetStream();
//                 byte[] buffer = new byte[1024];
//                 int byteCount = stream.Read(buffer, 0, buffer.Length);

//                 if(byteCount == 0)
//                 {
//                     break;
//                 }

//                 string data = Encoding.ASCII.GetString(buffer, 0, byteCount);
//                 // send data to all clients
//                 Broacast(data);
//                 Console.WriteLine($"ClientId {clientId} is broacasting: {data}");
//             }

//             lock(_lock)
//             {
//                 clients.Remove(clientId);
//             }

//             client.Client.Shutdown(SocketShutdown.Both);
//             client.Close();
//         }

//         public static void Broacast(string message)
//         {
//             byte[] buffer = Encoding.ASCII.GetBytes(message + Environment.NewLine);

//             lock(_lock)
//             {
//                 foreach(TcpClient client in clients.Values)
//                 {
//                     NetworkStream stream = client.GetStream();
//                     stream.Write(buffer, 0, buffer.Length);
//                 }
//             }
//         }
//     }
// }