using System.Text;
using System.Net;
using System.Net.Sockets;
using System;
using System.Globalization;

public class SocketTest {

    private static string RENDEZVOUS_SERVER_IP = "52.15.150.22";
    private static int RENDEZVOUS_SERVER_PORT = 8050;

    static void Main() {
        string peerIp;
        int peerPort;

        int localPort = 19375;
        UdpClient client = new UdpClient(localPort);
        Console.WriteLine("created socket");

        //IPEndPoint object will allow us to read datagrams sent from any source.
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);

        try {
            // connect to server
            Byte[] sendBytes = Encoding.ASCII.GetBytes("hello");

            client.Send(sendBytes, sendBytes.Length, RENDEZVOUS_SERVER_IP, RENDEZVOUS_SERVER_PORT);
            Console.WriteLine("sent data to server");

            Byte[] receiveBytes = client.Receive(ref ipEndPoint); 
            string returnData = Encoding.ASCII.GetString(receiveBytes);
            Console.WriteLine("recieved data from server");

            // parse peer address from server response
            string[] splitAddress = returnData.Split(null);
            peerIp = splitAddress[0];
            peerPort = Int32.Parse(splitAddress[1], NumberStyles.Integer);
            Console.WriteLine("peer address is: " + peerIp + " " + peerPort);

            // connect to peer
            client.Send(sendBytes, sendBytes.Length, peerIp, peerPort);
            Console.WriteLine("sent data to peer");

            receiveBytes = client.Receive(ref ipEndPoint);
            string peerResponse = Encoding.ASCII.GetString(receiveBytes);
            Console.WriteLine("recieved " + peerResponse + " from peer");

            // start thread to recieve peer data
            //Thread listenThread = new Thread(() => ListenThread(peerResponse));
            //listenThread.Start();

            client.Close();
        } catch (Exception e ) {
            Console.WriteLine("Exception!");
            Console.WriteLine(e.ToString());
        }
    }

    public void ListenThread(String addr, int port) {
        return;
    }
}