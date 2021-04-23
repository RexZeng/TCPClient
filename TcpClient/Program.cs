using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpClient
{
    class Program
    {
        static Socket client;
        static byte[] receiveData = new byte[1024];
        public object Enconding { get; private set; }

        static void Main(string[] args)
        {
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 100);
                IPEndPoint edp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000);
                try
                {
                    client.Connect(edp);
                    client.BeginReceive(receiveData, 0, receiveData.Length, SocketFlags.None, ReceiveCallBack, client);
                    bool bQuited = false;
                    while (!bQuited)
                    {
                        string str = Console.ReadLine();
                        str = str.Replace("\r", "").Replace("\n", "");
                        if (str.Length == 0) continue;
                        if (str.ToLower() == "q" || str.ToLower() == "exit") bQuited = true;
                        else
                        {
                            byte[] data = Encoding.Default.GetBytes(str);
                            client.Send(data);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("connect sever error: {0}", e.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (client != null)
                {
                    if (client.Connected) client.Disconnect(false);
                    client = null;
                }
                Console.Read();
            }
        }
        static void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                Socket client = ar.AsyncState as Socket;
                int cnt = client.EndReceive(ar);
                if (cnt > 0)
                {
                    string str = Encoding.ASCII.GetString(receiveData, 0, cnt);
                    Console.WriteLine(str);
                }
                if (client.Connected) client.BeginReceive(receiveData, 0, receiveData.Length, SocketFlags.None, ReceiveCallBack, client);
                else Console.WriteLine("[MSG]>>> disconnected");
            }
            catch (Exception e)
            {
                Console.WriteLine("[Error]>>>{0}", e.Message);
            }
        }
    }
}
