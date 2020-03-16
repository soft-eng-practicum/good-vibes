using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    NetworkManager netMan;
    string data = "dab";

    public override void OnStartServer()
    {
        netMan = GameObject.Find("DefaultNetworkManager").GetComponent<NetworkManager>();
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("Player.cs - client connected");
    }

    
}
