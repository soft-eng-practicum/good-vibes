using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CheckSingleReply : MonoBehaviour
{

    public Text topic;
    public InputField inputField;
    public GameObject inputs;
    public GameObject scrollView;
    public Button postMsg;
    public string button;
    public Canvas replyCanvas;

    // Start is called before the first frame update
    void Start()
    {
        replyCanvas = GameObject.Find("ReplyCanvas").GetComponent<Canvas>();
        postMsg = GameObject.Find("PostMsg").GetComponent<Button>();
        inputs = GameObject.Find("reply");
        topic = GameObject.Find("topic").GetComponent<Text>();
        inputField = GameObject.Find("InputField").GetComponent<InputField>();
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
