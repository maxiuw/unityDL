using System.Net.Sockets;
using System;
using System.Threading;

namespace unityTCPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // creating and starting new threads 
            new Thread(() => {Thread.CurrentThread.IsBackground = true; 
            ConnectClient("127.0.0.1",13000, $"client ID {1} sending a message...");
            }).Start();
            new Thread(() => {Thread.CurrentThread.IsBackground = true; 
            ConnectClient("127.0.0.1",13000, $"client ID {2} sending a message...");
            }).Start();
            new Thread(() => {Thread.CurrentThread.IsBackground = true; 
            ConnectClient("127.0.0.1",13000, $"client ID {3} sending a message...");
            }).Start();
            new Thread(() => {Thread.CurrentThread.IsBackground = true; 
            ConnectClient("127.0.0.1",13000, $"client ID {4} sending a message...");
            }).Start();
            Console.ReadLine(); // waiting for the console to open 
        }
    

        // attempt a connection with the server 
        // takes an ip address of the server
        // and message we are sending to the server 
        private static void ConnectClient(string server, int port, string message) {
            // try to catch any error (generoc)
            try {
                // specify port we want to connect ot
                // create the instance TcpClient we created last time
                TcpClient client = new TcpClient(server, port);
                NetworkStream stream = client.GetStream();
                // count the nu,ber of the messages 
                int count = 0; 
                while (count++ < 3) {
                    // wait until 3 msgs are recieved 
                    // conv mesg to bytes  
                    // ascii cause the smae thing we do in the server
                    byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine($"mesage: {message} sent!");
                    data = new byte[256];
                    // get bytes from the response 
                    // reading from the stream - server is sending us the information
                    int bytes = stream.Read(data, 0, data.Length);
                    // make them readable -> decoding
                    string response = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    Console.WriteLine($"recieved: {response}");
                    // sleep (wait for the response) 3 sec
                    Thread.Sleep(2000);
                } 
                
                // close stream
                stream.Close();
                client.Close();
            }
            catch(Exception e) {
                Console.WriteLine($"exception ({e}");
            }
            Console.Read();
        }
    }

    
}
