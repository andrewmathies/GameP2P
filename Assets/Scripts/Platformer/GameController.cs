using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject curPlayer, playerPrefab, mainCamera;

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
        // open socket, send message to server
        // do same stuff as python client, send message to address found in server response
        // serialize player x,y and state into string, send through socket
        // start thread to listen on socket for message, deserialize other player data
        // move around player using deserialized data
    }
}
