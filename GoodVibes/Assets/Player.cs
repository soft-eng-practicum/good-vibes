using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    NetworkManager netMan;

    public override void OnStartServer()
    {
        netMan = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("Player.cs - client connected");
    }
}
