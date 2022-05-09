using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Threading;
using TMPro;
public class TCPClient : MonoBehaviour
{
    
    // Start is called before the first frame update
    [SerializeField]
    private string serverIPAddress = "127.0.0.1";
    [SerializeField]
    private int port = 13000;
    [SerializeField]
    private int clients = 1;
    [SerializeField]
    private TextMeshProUGUI outputText;
    void Awake() {
        // create dispatcher before threads got created 
        // not sure why we need that... 
        Dispatcher disp = Dispatcher.Instance; 
    }
    void Start()
    {
        for (int i = 1; i <= clients; i++) {
            new Thread(() => {Thread.CurrentThread.IsBackground = true; 
        ConnectClient("127.0.0.1",13000, $"client ID {i} sending a message...", i);
        }).Start();
        }
        
        
    }

    // recieving message
    private void Message(int clientID, string message)
    {
        Debug.Log($"ClientID: {clientID}");
        Debug.Log($"message: {message}");
        outputText.text = $"ClientID: {clientID}: message: {message}\n";
    }

    // if method is static it cannot access the other methods 
    private void ConnectClient(string server, int port, string message, int clientID) {
            // try to catch any error (generoc)
            try {
                // specify port we want to connect ot
                // create the instance TcpClient we created last time
                TcpClient client = new TcpClient(server, port);
                NetworkStream stream = client.GetStream();
                // count the nu,ber of the messages 
                // int count = 0; 
                while (true) {
                    // wait until 3 msgs are recieved 
                    // conv mesg to bytes  
                    // ascii cause the smae thing we do in the server
                    byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    // 'send' the message to the dispatecher 
                    // singletone (dispatcher) does not get created until we call the next line 
                    Dispatcher.Instance.Enqueue(() => Message(clientID, message));
                    data = new byte[256];
                    // get bytes from the response 
                    // reading from the stream - server is sending us the information
                    int bytes = stream.Read(data, 0, data.Length);
                    // make them readable -> decoding
                    string response = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    // send the repsonse to the dispather 
                    Dispatcher.Instance.Enqueue(() => Message(clientID, response));

                    // sleep (wait for the response) 3 sec
                    Thread.Sleep(2000);
                } 
                
                // close stream
                stream.Close();
                client.Close();
            }
            catch(Exception e) {
                // In case of exception the message will be the exp 
                Dispatcher.Instance.Enqueue(() => Message(clientID, e.Message));
            }
            Console.Read();
        }
}
