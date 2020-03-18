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
        #region old auto code
        //if platform = linux, start as server
        if (Application.platform == RuntimePlatform.WindowsEditor)
            NetworkServer.Listen(7777);
        //if platofrm = not linux, start as client (for windows/android)
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Debug.Log("Trying to connect...");
            myClient = new NetworkClient();
            myClient.RegisterHandler(MsgType.Connect, OnConnected);
            //myClient.Connect("157.245.127.176", 4444);
            myClient.Connect("127.0.0.1", 7777);
        }
        #endregion
    }

    public void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("Connected to server");
        myClient.RegisterHandler(MsgType.AddPlayer, OnAddPlayer);
        Debug.Log("Player wants to be spawned");
        //ClientDoSomething();
    }

    public void OnAddPlayer(NetworkMessage netMsg)
    {
        Debug.Log("Player wants to be spawned");
        GameObject thePlayer = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(myClient.connection, thePlayer, 0);
        NetworkServer.RegisterHandler(MsgType.Owner, OnPlayerAdded);
    }

    public void OnPlayerAdded(NetworkMessage netMsg)
    {
        Debug.Log("Player GameObject added");
    }

    /*public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
    }*/

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

    /*[Command]
    void ClientDoSomething()
    {
        PleaseSpawnMe();
    }

    void PleaseSpawnMe()
    {
        SpawnPlayer();
    }

    [ClientRpc]
    void SpawnPlayer()
    {
        Instantiate(this.spawnPrefabs[0]);
    }*/
}
