using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server_HW1
{
    internal class Program
    {
        static void Main(string[] args)
        {
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

        }
    }
}
