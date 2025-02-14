using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client_PT1
{
     internal class Program  // ===== CLIENT ====
    {
        static void Main(string[] args)
        {
            //here we establish ip and port of server client's gonna connect to 
            string ipAddress = "127.0.0.1";
            int port = 12345;

            //creating client Socket                           IPv4               Type: stream     TCP 
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Connecting to the server (server here made with EndPoint as well)
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ipAddress), port));         // CLIENT TRY TO ESTABLISH CONNECTION WITH SERVER
            Console.WriteLine("Connected to server...");

            //string we're gonna send to the server. It converted in bytes ( byte array)
            string greeting = "hi, client!";
            clientSocket.Send(Encoding.ASCII.GetBytes(greeting));

            //Getting message from the server 
            byte[] buffer = new byte[1024];
            
            //read data from client socket and convert them to string 
            int receivedBytes = clientSocket.Receive(buffer);
            string response = Encoding.ASCII.GetString(buffer, 0, receivedBytes);

            Console.WriteLine($"Client:\nAt {DateTime.Now:HH:mm} from {clientSocket.RemoteEndPoint} was given a string: {response}");

            //Shutting connection 
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }
}
