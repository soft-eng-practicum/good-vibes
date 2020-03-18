using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections.Generic;

public class AutoCreateMatch : MonoBehaviour
{
    bool matchCreated;
    //NetworkMatch matchMaker;
    NetworkManager netMan;

    void Awake()
    {
        if (Application.platform != RuntimePlatform.WindowsEditor)
            this.enabled = false;
        //matchMaker = gameObject.AddComponent<NetworkMatch>();
    }

    // Start is called before the first frame update
    void Start()
    {
        netMan = GameObject.Find("NetMan").GetComponent<NetworkManager>();
        NetworkManager.singleton.StartMatchMaker();
        NetworkManager.singleton.matchMaker.CreateMatch("GoodVibesServer", 20, true, "", "10.0.0.199", "", 0, 0, OnMatchCreate);
        //CreateMatch("GoodVibesServer", 20, true, "", "10.0.0.199", "", 0, 0, OnMatchCreate);
        //netMan.SetMatchHost("GoodVibesServer", 4444, true);
        //netMan.StartMatchMaker();
        //netMan.StartServer(netMan.connectionConfig, 4);
    }

    public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Debug.Log("Create match succeeded");
            matchCreated = true;
            Utility.SetAccessTokenForNetwork(matchInfo.networkId, matchInfo.accessToken);
            NetworkServer.Listen(matchInfo, 4444);
        }
        else
        {
            Debug.LogError("Create match failed");
        }
    }
}
