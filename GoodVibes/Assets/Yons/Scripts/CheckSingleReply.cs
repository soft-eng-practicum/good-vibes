using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CheckSingleReply : PlayerController
{

    private Text topic;
    private InputField inputField;
    private GameObject inputs;
    private GameObject scrollView;
    private Button postReplyMsg;
    private string button;
    private Canvas replyCanvas;
    private Canvas messagesCanvas;
    private GameObject playerPrefab;
    private GameObject updateMsg;

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Debug.Log(scrollView.activeInHierarchy);
            Debug.Log(inputs.activeInHierarchy);
        }*/

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            replyCanvas = GameObject.Find("ReplyCanvas").GetComponent<Canvas>();
            messagesCanvas = GameObject.Find("TopicsCanvas").GetComponent<Canvas>();
            postReplyMsg = GameObject.Find("PostMsg").GetComponent<Button>();
            inputs = GameObject.Find("reply");
            topic = GameObject.Find("topic").GetComponent<Text>();
            inputField = GameObject.Find("ReplyInputField").GetComponent<InputField>();
            scrollView = GameObject.Find("Scroll View");
            updateMsg = GameObject.Find("UpdateMsg");

            //inputs.SetActive(false);
            messagesCanvas.enabled = false;
            replyCanvas.enabled = false;

            postReplyMsg.onClick.AddListener(postReply);
            //topics = GameObject.FindGameObjectsWithTag
        }
    }

    public void postReply()
    {
        CmdPostReply(inputField.text, clawmail);
    }

    [Command]
    public void CmdPostReply(string replyField, string clawmail)
    {
        if (isServer)
            Debug.Log("post reply has been pressed");
        StartCoroutine(Reply(replyField, clawmail));
    }

    IEnumerator Reply(string replyField, string clawmail)
    {
        Debug.Log("post reply to topic vibe (www) coroutine started on server");
        WWWForm replyForm = new WWWForm();
        replyForm.AddField("reply", replyField);
        replyForm.AddField("clawmail", clawmail);
        replyForm.AddField("messageID", 1);
        WWW www = new WWW("http://localhost/sql/sendReply.php", replyForm);
        yield return www;

        if (www.text == "0") //no errors
        {
            string update = "reply added successfully.";
            Debug.Log(update);
            RpcShowUpdateTest(update);
        }
        //Debug.Log("Server connected to DB");
    }

    [ClientRpc]
    void RpcShowUpdateTest(string update)
    {
        updateMsg.GetComponent<Text>().text = "" + update;
    }

    public void topicClicked()
    {
        replyCanvas = GameObject.Find("ReplyCanvas").GetComponent<Canvas>();
        replyCanvas.enabled = true;
        topic = GameObject.Find("topic").GetComponent<Text>();
        scrollView = GameObject.Find("Scroll View");
        button = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(button);
        Debug.Log("tc" + scrollView.activeInHierarchy);
        string txt = GameObject.Find(button).GetComponentInChildren<Text>().text;
        topic.text = txt;
        scrollView.SetActive(false);
    }

    public void displayMessagesCanvas()
    {
        messagesCanvas = GameObject.Find("TopicsCanvas").GetComponent<Canvas>();
        messagesCanvas.enabled = true;
    }

    
}
