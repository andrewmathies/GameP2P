using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System;
using System.Globalization;
using System.Threading;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject curPlayer, playerPrefab, mainCamera;

    private static string RENDEZVOUS_SERVER_IP = "52.15.150.22";
    private static int RENDEZVOUS_SERVER_PORT = 8050;

    public void Reset()
    {
        Destroy(curPlayer);
        curPlayer = Instantiate(playerPrefab, new Vector2(-1.5f, -15.5f), Quaternion.identity);
        mainCamera.GetComponent<CameraController>().UpdatePlayerReference(curPlayer.transform);
    }

    public void EndGame()
    {
        Application.Quit();
    }

    public void Start() {
        string peerIp;
        int peerPort;

        UdpClient client = new UdpClient();
        Debug.Log("created socket");

        //IPEndPoint object will allow us to read datagrams sent from any source.
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);

        try {
            // connect to server
            Byte[] sendBytes = Encoding.ASCII.GetBytes("hello");

            client.Send(sendBytes, sendBytes.Length, RENDEZVOUS_SERVER_IP, RENDEZVOUS_SERVER_PORT);
            Debug.Log("sent data to server");

            Byte[] receiveBytes = client.Receive(ref ipEndPoint); 
            string returnData = Encoding.ASCII.GetString(receiveBytes);
            Debug.Log("recieved data from server");

            // parse peer address from server response
            string[] splitAddress = returnData.Split(null);
            peerIp = splitAddress[0];
            peerPort = Int32.Parse(splitAddress[1], NumberStyles.Integer);
            Debug.Log("peer address is: " + peerIp + " " + peerPort);

            // start thread to recieve peer data
            StartCoroutine(Send(client, peerIp, peerPort));
            StartCoroutine(Listen(client, peerIp, peerPort));

            client.Close();
        } catch (Exception e ) {
            Debug.Log("Exception!");
            Debug.Log(e.ToString());
        }

        // open socket, send message to server
        // do same stuff as python client, send message to address found in server response
        // serialize player x,y and state into string, send through socket
        // start thread to listen on socket for message, deserialize other player data
        // move around player using deserialized data
    }

    private IEnumerator Send(UdpClient socket, String addr, int port) {
        while (true) {
            Vector2 pos = curPlayer.transform.position;
            String msg = pos.ToString("F3");
            Byte[] sendBytes = Encoding.ASCII.GetBytes(msg);
            socket.Send(sendBytes, sendBytes.Length, addr, port);
            yield return new WaitForFixedUpdate();
        } 
    }

    private IEnumerator Listen(UdpClient socket, String addr, int port) {
        socket.Client.ReceiveTimeout = 2000;

        while (true) {
            IPEndPoint peerEndPoint = new IPEndPoint(IPAddress.Parse(addr), port);
            Byte[] receiveBytes = socket.Receive(ref peerEndPoint);
            string peerResponse = Encoding.ASCII.GetString(receiveBytes);
            Debug.Log("recieved " + peerResponse + " from peer");
        }
    }
}
