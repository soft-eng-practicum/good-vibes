using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

#pragma warning disable CS0618 // Type or member is obsolete
public class PlayerController : NetworkBehaviour
{

    NetworkManager netMan;
    PostDB db;

    InputField myField;
    Text postPanel;
    

    public override void OnStartServer()
    {
        netMan = GameObject.Find("NetMan").GetComponent<NetworkManager>();
        db = netMan.GetComponent<PostDB>();

    }

    public override void OnStartLocalPlayer()
    {
        myField = GameObject.Find("InputField").GetComponent<InputField>();
        postPanel = GameObject.Find("Posts").GetComponent<Text>();

        GameObject.Find("PostMsg").GetComponent<Button>().onClick.AddListener(postMsg);
        GameObject.Find("GetPosts").GetComponent<Button>().onClick.AddListener(loadPosts);
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
