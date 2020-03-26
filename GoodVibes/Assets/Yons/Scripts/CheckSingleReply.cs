using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CheckSingleReply : PlayerController
{

    private Text topic;
    private InputField inputField;
    private GameObject inputs;
    private GameObject scrollView;
    private Button postMsg;
    private string button;
    private Canvas replyCanvas;
    private GameObject playerPrefab;


    // Start is called before the first frame update
    void Start()
    {
        replyCanvas = GameObject.Find("ReplyCanvas").GetComponent<Canvas>();
        postMsg = GameObject.Find("PostMsg").GetComponent<Button>();
        inputs = GameObject.Find("reply");
        topic = GameObject.Find("topic").GetComponent<Text>();
        inputField = GameObject.Find("ReplyInputField").GetComponent<InputField>();
        scrollView = GameObject.Find("Scroll View");

        //inputs.SetActive(false);
        replyCanvas.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log(scrollView.activeInHierarchy);
        Debug.Log(inputs.activeInHierarchy);

    }

    [Command]
    public void CmdCallRegister()
    {
        if (isLocalPlayer)
            return;
        StartCoroutine(Register(inputField, clawmail));
    }

    IEnumerator Register(InputField replyField, string clawmail)
    {
        WWWForm replyForm = new WWWForm();
        replyForm.AddField("reply", replyField.text);
        replyForm.AddField("clawmail", clawmail);
        WWW www = new WWW("http://localhost/sql/register.php", replyForm);
        yield return www;
        Debug.Log("Server connected to DB");
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

    
}
