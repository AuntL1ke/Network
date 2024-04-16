using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace client_socket
{
    internal class Program
    {
        static string address = "127.0.00.1";
        static int port = 8080;
        static void Main(string[] args)
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                IPEndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);

                //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                string message = "";
                UdpClient client = new UdpClient();

                while (message!="end")
                {
                    Console.Write("Enter a message ::");
                    message = Console.ReadLine();
                    byte[] data = Encoding.Unicode.GetBytes(message);
                  //  socket.SendTo(data,ipPoint);
                    client.Send(data,data.Length,ipPoint);

                    int bytes = 0;
                    string response="";
                    data = new byte[1024];
                    /* do
                     {
                         bytes = socket.ReceiveFrom(data, ref remotePoint);
                         response += Encoding.Unicode.GetString(data);

                     } while (socket.Available > 0);
                     */
                    data = client.Receive(ref remotePoint);
                    response = Encoding.Unicode.GetString(data);
                    Console.WriteLine("server response ::" + response);
                    
                }
               /* socket.Shutdown(SocketShutdown.Both);
                socket.Close();*/
               client.Close();
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
