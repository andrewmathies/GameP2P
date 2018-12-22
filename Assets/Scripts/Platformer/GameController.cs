using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System;
using System.Globalization;
using System.Threading;
using UnityEngine;
using System.Text.RegularExpressions;

public class GameController : MonoBehaviour {

    private static string RENDEZVOUS_SERVER_IP = "52.15.150.22";
    private static int RENDEZVOUS_SERVER_PORT = 8050;

    public GameObject curPlayer, playerPrefab, mainCamera, opponent;

    private string peerIp;
    private int peerPort;

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
        peerPort = -1;

        UdpClient client = new UdpClient();
        Debug.Log("created socket");

        // thread to get peer addr from rendezvous server
        StartCoroutine(ConnectToPeer(client));

        // start threads to send and recieve data to and from peer
        StartCoroutine(Send(client));
        StartCoroutine(Listen(client));
    }

    private IEnumerator ConnectToPeer(UdpClient client) {
        //IPEndPoint object will allow us to read datagrams sent from any source.
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);

        try {
            // connect to server
            Byte[] sendBytes = Encoding.ASCII.GetBytes("hello");
            Byte[] receiveBytes;

            client.Send(sendBytes, sendBytes.Length, RENDEZVOUS_SERVER_IP, RENDEZVOUS_SERVER_PORT);
            Debug.Log("sent data to server, waiting for response");

            while (true) {
                if (client.Available > 0) {
                    receiveBytes = client.Receive(ref ipEndPoint);
                    break;
                }

                yield return null;
            }

            string returnData = Encoding.ASCII.GetString(receiveBytes);
            Debug.Log("recieved data from server");

            // parse peer address from server response
            string[] splitAddress = returnData.Split(null);
            peerIp = splitAddress[0];
            peerPort = Int32.Parse(splitAddress[1], NumberStyles.Integer);
            Debug.Log("peer address is: " + peerIp + " " + peerPort);
        } finally {
            // not sure why I need this, but compiler complained about yield return null if this wasn't here
            Debug.Log("done");
        }

        yield return new WaitForFixedUpdate();
    }

    private IEnumerator Send(UdpClient socket) {
        while (peerPort == -1)
            yield return null;

        Debug.Log("started sending on peer connection");

        while (true) {
            Vector2 pos = curPlayer.transform.position;
            String msg = pos.ToString("F3");
            Byte[] sendBytes = Encoding.ASCII.GetBytes(msg);
            socket.Send(sendBytes, sendBytes.Length, peerIp, peerPort);
            yield return new WaitForFixedUpdate();
        } 
    }

    private IEnumerator Listen(UdpClient socket) {
        while (peerPort == -1)
            yield return null;

        Debug.Log("started listening on peer connection");
        Byte[] receiveBytes;

        while (true) {
            IPEndPoint peerEndPoint = new IPEndPoint(IPAddress.Parse(peerIp), peerPort);

            while (true) {
                if (socket.Available > 0) {
                    receiveBytes = socket.Receive(ref peerEndPoint);
                    break;
                }

                yield return null;
            }

            string peerResponse = Encoding.ASCII.GetString(receiveBytes);
            Debug.Log("recieved " + peerResponse + " from peer");
            
            // parsing position from response
            MatchCollection mc = Regex.Matches(peerResponse, @"-?\d+.\d{3}");
            var matches = new string[mc.Count];
            for (int i = 0; i < matches.Length; i++)
                matches[i] = mc[i].ToString();

            float x = 0f, y = 0f;

            try {
                x = float.Parse(matches[0]);
                y = float.Parse(matches[1]);
                opponent.transform.position = new Vector2(x, y);
            } catch (Exception e) {
                Debug.Log(e.ToString());
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
