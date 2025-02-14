using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;


namespace NetworkClientServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // intializing port and ip for server
            string ipAddress = "127.0.0.1";
            int port = 12345;

            // creating socket                  Ipv4                        Type:Stream  TCP
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //wiring ip and port of server (With object EndPoint) and giving it to the socket 
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            serverSocket.Bind(ipEndPoint);

            //Socket listens to connections( here he can get only one connection)
            serverSocket.Listen(1);
            Console.WriteLine("Waiting for client connection...");

            //Socket is waiting for client connectiom

            //When client's connected, there is a new socket created (to use it for exchanging data with the client)
            Socket clientSocket = serverSocket.Accept();              // SERVER ACCEPTS THIS CONNECTION.
            Console.WriteLine("Client is connected: " + clientSocket.RemoteEndPoint.ToString());   

            //Server uses another socket(clientSocket) to communicate with certain client

            //Getting message from the client: 

            //buffer for storing received data
            byte[] buffer = new byte[1024];

            //receive() - reads data from socket and send it to buffer( in bytes) 
            int receivedBytes = clientSocket.Receive(buffer);

            //Here we convert bytes to string
            string receivedMessage = Encoding.ASCII.GetString(buffer, 0, receivedBytes);

            //Output of given message
            Console.WriteLine($"Server:\nAt {DateTime.Now:HH:mm} from {clientSocket.RemoteEndPoint} was given a string: {receivedMessage}");

            //Sending data to client socket
            string response = "hi, client!";
            clientSocket.Send(Encoding.ASCII.GetBytes(response));

            //Shutting the connection 
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            serverSocket.Close();
        }
    }
}
