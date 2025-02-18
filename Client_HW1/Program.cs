using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Client_HW1
{
    internal class Program
    {
        static void Main(string[] args)
        {

#if true1
            string servAD = "127.0.0.1";
            int port = 11000;
            UdpClient udpClient = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(servAD), port);

            Console.WriteLine("Client is connected. See queries:\ntime\nexit ");

            while (true)
            {
                Console.Write("query: ");
                string query = Console.ReadLine();
                if (query == "exit") break;

                byte[] bytes = Encoding.UTF8.GetBytes(query);
                udpClient.Send(bytes, bytes.Length, endPoint);

                byte[] response = udpClient.Receive(ref endPoint);
                Console.WriteLine($"Server: {Encoding.UTF8.GetString(response)}");
            }
            udpClient.Close();
            udpClient.Dispose(); 
#endif
            //FOR CHAT 

            
            string IpAdd = "127.0.0.1"; // for server 
            int port = 1202;

            Socket clientToServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientToServer.Connect(new IPEndPoint(IPAddress.Parse(IpAdd), port));
            Console.WriteLine("Client is connected");

          
            Console.Write("Enter ip: "); string ip = Console.ReadLine();
            Console.Write("Enter port: "); string toport = Console.ReadLine();
            Console.Write("message: "); string message = Console.ReadLine();
            clientToServer.Send(Encoding.ASCII.GetBytes($"{ip}\\{toport}\\{message}"));

            byte[] buffer = new byte[1024];     
            int receivedBytes = clientToServer.Receive(buffer);
            string response = Encoding.ASCII.GetString(buffer, 0, receivedBytes);

            Console.WriteLine($"{response}");
        }
    }
}
