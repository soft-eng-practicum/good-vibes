using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkHelper : MonoBehaviour
{
    public NetworkManager manager;

    void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.LinuxPlayer)
        {
            if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
            {
                manager.StartServer();
            }

            if (!NetworkServer.active && !NetworkClient.active)
            {
                if (manager.matchMaker == null)
                {
                    manager.StartMatchMaker();
                }
                else
                {
                    if (manager.matchInfo == null)
                    {
                        if (manager.matches == null)
                        {
                            manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "GoodVibes420", "", "", 0, 0, manager.OnMatchCreate);
                        }
                    }
                }
            }
        }
        else 
        {
            if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
            {
                manager.StartClient();
            }

            if (!NetworkServer.active && !NetworkClient.active)
            {
                if (manager.matchMaker == null)
                {
                    manager.StartMatchMaker();
                }
                else
                {
                    if (manager.matchInfo == null)
                    {
                        if (manager.matches != null)
                        {
                            foreach (var match in manager.matches)
                            {
                                manager.matchName = match.name;
                                manager.matchSize = (uint)match.currentSize;
                                manager.matchMaker.JoinMatch(match.networkId, "GoodVibes420", "", "", 0, 0, manager.OnMatchJoined);
                            }
                        }
                    }
                }
            }
        }
    }
}