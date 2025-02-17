using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client_HW1
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string servAD = "127.0.0.1";
            int port = 11000;
            UdpClient udpClient = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(servAD), port);

            Console.WriteLine("Client is connected. See queries:\ntime\nexit ");

            while(true)
            {
                Console.Write("query: ");
                string query = Console.ReadLine();
                if(query == "exit")break;

                byte[] bytes = Encoding.UTF8.GetBytes(query);
                udpClient.Send(bytes, bytes.Length, endPoint);

                byte[] response = udpClient.Receive(ref endPoint);
                Console.WriteLine($"Server: {Encoding.UTF8.GetString(response)}");   
            }
            udpClient.Close();
            udpClient.Dispose();
        }
    }
}
