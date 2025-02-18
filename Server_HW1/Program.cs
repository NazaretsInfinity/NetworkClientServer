using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Serialization;

namespace Server_HW1
{
    internal class Program
    {
        static void Main(string[] args)
        {
#if true1
            int port = 11000;
            UdpClient udpServer = new UdpClient(port);
            Console.WriteLine("Server is active");
            IPEndPoint RemotePoint = new IPEndPoint(IPAddress.Any, port);

            while (true)
            {
                try
                {
                    byte[] buffer = udpServer.Receive(ref RemotePoint);
                    string receivedMES = Encoding.UTF8.GetString(buffer);
                    if (receivedMES == "time")
                    {
                        byte[] response = Encoding.UTF8.GetBytes(DateTime.Now.ToString());
                        udpServer.Send(response, response.Length, RemotePoint);
                    }
                    else Console.WriteLine("Incorrect query");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            //udpServer.Close();



#endif
            //SERVER FOR CHAT
            string IpAdd = "127.0.0.1";
            int port = 1202;
           

            Socket TransSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            TransSocket.Bind(new IPEndPoint(IPAddress.Parse(IpAdd), port));
            Console.WriteLine("Server is online.");

            TransSocket.Listen(2);
            while (true)
            {
                Socket client = TransSocket.Accept();
                Console.WriteLine($"Client is connected: {client.RemoteEndPoint}");
                Thread handleClient = new Thread(() => HandleClient(client)); handleClient.Start();
                Thread.Sleep(5000);
            }


           
        }

        static void HandleClient(Socket socket)
        {

            byte[] buffer = new byte[1024];

            try
            {
                while (true)
                {
                    try
                    {
                        
                        int resBytes = socket.Receive(buffer);                          // send several packets
                        string[] ForRecipient = Encoding.UTF8.GetString(buffer, 0, resBytes).Split('\\');

                        if (ForRecipient.Contains("EXIT")) break;

                        if (ForRecipient.Contains("SendMessage"))
                        {
                            //whom send
                            Socket friend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            friend.Connect(new IPEndPoint(IPAddress.Parse(ForRecipient[0]), Int32.Parse(ForRecipient[1])));
                            friend.Send(Encoding.ASCII.GetBytes(ForRecipient[2]));
                            Console.WriteLine($" Client {socket.RemoteEndPoint} sent a message to {friend.RemoteEndPoint}\n {ForRecipient[2]}");
                            friend.Shutdown(SocketShutdown.Both); friend.Close(); friend.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        break; 
                    }
                }
            }
            finally
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
    }
}
