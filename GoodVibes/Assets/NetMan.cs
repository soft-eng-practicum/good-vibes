using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetMan : NetworkManager
{
    public bool isAtStartup = true;
    NetworkClient myClient;

    // Start is called before the first frame update
    void Start()
    {
        //if platform = linux, start as server
        if (Application.platform == RuntimePlatform.LinuxPlayer)
            NetworkServer.Listen(4444);
        //if platofrm = not linux, start as client (for windows/android)
        /*if (Application.platform != RuntimePlatform.LinuxPlayer)
        {
            Debug.Log("Trying to connect...");
            myClient = new NetworkClient();
            myClient.RegisterHandler(MsgType.Connect, OnConnected);
            myClient.Connect("157.245.127.176", 4444);
        }*/
    }
    public void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("Connected to server");
    }

    // Client callbacks
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log("Connected successfully to server, now to set up other stuff for the client...");
    }

    // Server callbacks
    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("A client connected to the server: " + conn);
        System.Console.WriteLine("A client connected to the server: " + conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        NetworkServer.DestroyPlayersForConnection(conn);
        if (conn.lastError != NetworkError.Ok)
        {
            if (LogFilter.logError) { Debug.LogError("ServerDisconnected due to error: " + conn.lastError); }
        }
        Debug.Log("A client disconnected from the server: " + conn);
        System.Console.WriteLine("A client disconnected from the server: " + conn);
    }
}
