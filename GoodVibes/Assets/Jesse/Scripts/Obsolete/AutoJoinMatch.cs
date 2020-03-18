using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections.Generic;

public class AutoJoinMatch : NetworkBehaviour
{
    #region nah
    /*NetworkMatch matchMaker;

    // Start is called before the first frame update
    void Start()
    {
        matchMaker = gameObject.AddComponent<NetworkMatch>();
    }

    public void OnMatchList(ListMatchResponse matchListResponse)
    {
        if (matchListResponse.success && matchListResponse.matches != null)
        {
            matchMaker.JoinMatch(networkId, "", "", "", 0, 0, OnMatchJoined);
        }
    }

    public void OnMatchJoined(JoinMatchResponse matchJoin)
    {
        if (matchJoin.success)
        {
            Debug.Log("Join match succeeded");
            if (matchCreated)
            {
                Debug.LogWarning("Match already set up, aborting...");
                return;
            }
            Utility.SetAccessTokenForNetwork(matchJoin.networkId, new NetworkAccessToken(matchJoin.accessTokenString));
            NetworkClient myClient = new NetworkClient();
            myClient.RegisterHandler(MsgType.Connect, OnConnected);
            myClient.Connect(new MatchInfo(matchJoin));
        }
        else
        {
            Debug.LogError("Join match failed");
        }
    }

    public void OnConnected(NetworkMessage msg)
    {
        Debug.Log("Connected!");
    }*/
    #endregion

    NetworkManager netMan;

    void Awake()
    {
        if (Application.platform != RuntimePlatform.WindowsPlayer)
            this.enabled = false;
    }

    private void Start()
    {
        netMan = GameObject.Find("DefaultNetworkManager").GetComponent<NetworkManager>();
        netMan.StartClient(netMan.matchInfo, netMan.connectionConfig, netMan.matchPort);
    }

    public virtual void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        var player = (GameObject)GameObject.Instantiate(netMan.playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
}
