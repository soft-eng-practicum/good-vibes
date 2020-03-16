using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#pragma warning disable CS0618 // Type or member is obsolete
public class PlayerController : NetworkBehaviour
{

    NetworkManager netMan;
    PostDB db;

    InputField myField;
    Text postPanel;

    Text connecting;
    int timeLeft;

    private void Awake()
    {
        timeLeft = 3;
        print("timeLeft set");
    }

    public override void OnStartServer()
    {
        netMan = GameObject.Find("NetMan").GetComponent<NetworkManager>();
        db = netMan.GetComponent<PostDB>();
    }

    public override void OnStartLocalPlayer()
    {
        /*if (SceneManager.GetActiveScene().name == "ClientScene")
        {
            myField = GameObject.Find("InputField").GetComponent<InputField>();
            postPanel = GameObject.Find("Posts").GetComponent<Text>();

            GameObject.Find("PostMsg").GetComponent<Button>().onClick.AddListener(postMsg);
            GameObject.Find("GetPosts").GetComponent<Button>().onClick.AddListener(loadPosts);
        }*/

        if (SceneManager.GetActiveScene().name == "Disconnected")
        {
            connecting = GameObject.Find("Connecting").GetComponent<Text>();

            //ask for scene change
            CmdAskLoadConnectingScene();
        }

        if (SceneManager.GetActiveScene().name == "Connecting")
        {
            connecting = GameObject.Find("Connecting").GetComponent<Text>();
            
            //ask for scene change
            CmdAskLoadLogInScene();
        }

        if (SceneManager.GetActiveScene().name == "Login")
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }

    [Command]
    public void CmdAskLoadConnectingScene()
    {
        RpcClientLoadConnectingScene();
    }

    [Command]
    public void CmdAskLoadLogInScene()
    {
        RpcClientLoadLogInScene();
    }

    [ClientRpc]
    public void RpcClientLoadConnectingScene()
    {
        SceneManager.LoadScene("Connecting");
    }

    [ClientRpc]
    public void RpcClientLoadLogInScene()
    {
        StartCoroutine(ClientConnected());
    }

    IEnumerator ClientConnected()
    {
        print("timeLeft: " + timeLeft);
        connecting.text = "Connected! Login screen will show up in " + timeLeft + " seconds";
        yield return new WaitForSeconds(1);
        timeLeft--;
        if (timeLeft == 0)
            SceneManager.LoadScene("Login");
        else
            StartCoroutine(ClientConnected());
    }

    [Command]
    public void CmdPostMessage(string s)
    {
        db.addMsg(s);
    }

    public void postMsg()
    {
        if (isLocalPlayer)
            CmdPostMessage(myField.text);
    }

    [Command]
    public void CmdLoadPosts()
    {
        //send client a random list of posts (s)he has not yet responded too
        string[] postsToSend = new string[db.currentLen];
        for (int i = 0; i < postsToSend.Length; i++)
            postsToSend[i] = db.posts[i];
        RpcViewPosts(postsToSend);
    }

    public void loadPosts()
    {
        if (isLocalPlayer)
            CmdLoadPosts();
    }

    [ClientRpc]
    public void RpcViewPosts(string[] posts)
    {
        postPanel.text = "";
        for (int i = 0; i < posts.Length; i++)
        {
            postPanel.text += posts[i] + "\n";
        }
    }





}
