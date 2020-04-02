using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SendTopicVibe : NetworkBehaviour
{

    private Canvas topicCanvas;
    private InputField topicInputField;
    private InputField subjectInputField;
    private Text topic;
    private GameObject updateMsg; 
    private Button postTopicMsg;

    // Start is called before the first frame update
    void Start()
    {

        //add to on scene loaded
        topicCanvas = GameObject.Find("SendTopicCanvas").GetComponent<Canvas>();
        topicInputField = GameObject.Find("sendTopicMessageField").GetComponent<InputField>();
        subjectInputField = GameObject.Find("sendTopicSubjectField ").GetComponent<InputField>();
        topic = GameObject.Find("topic").GetComponent<Text>();
        updateMsg = GameObject.Find("SendTopicUpdateMsg");
        postTopicMsg = GameObject.Find("PostTopicMsg").GetComponent<Button>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region topic

    public void postTopic()
    {
        CmdPostTopic(topicInputField.text, subjectInputField.text, clawmail);
    }

    [Command]
    public void CmdPostTopic(string topicField, string subjectField, string clawmail)
    {
        if (isServer)
            Debug.Log("post reply has been pressed");
        StartCoroutine(Topic(topicField, subjectField,  clawmail));
    }

    IEnumerator Topic(string topicField, string subjectField, string clawmail)
    {
        Debug.Log("post reply to topic vibe (www) coroutine started on server");
        WWWForm topicReply = new WWWForm();
        topicReply.AddField("message", topicField);
        topicReply.AddField("subject", subjectField);
        topicReply.AddField("clawmail", clawmail);
        WWW www = new WWW("http://localhost/posttopicvibe.php", topicReply);
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
    #endregion


}
