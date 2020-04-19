using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CryptSharp;
using UnityEngine.EventSystems;

#pragma warning disable CS0618 // Type or member is obsolete
public class PlayerController : NetworkBehaviour
{
    #region demo stuff
    NetworkManager netMan;
    PostDB db;

    InputField myField;
    Text postPanel;
    #endregion

    Text connecting;
    int timeLeft;

    string msgHint;
    protected string clawmail;
    public WordFilter wf;

    MainMenuController mmc;

    #region delegates
    //Create an event delegate that will be used for creating methods that respond to events
    public delegate void EventDelegate(BaseEventData baseEvent);
    public void SelectEventMethod(UnityEngine.EventSystems.BaseEventData baseEvent)
    {
        Debug.Log(baseEvent.selectedObject.name + " triggered an event!");
        VerifyInputs();
        //baseEvent.selectedObject is the GameObject that triggered the event,
        // so we can access its components, destroy it, or do whatever.
    }
    #endregion

    #region registration
    [SerializeField] InputField clawmailField;
    [SerializeField] InputField passwordField;
    [SerializeField] Dropdown userType;
    [SerializeField] Button registerBtn;

    private string salt;
    private string hash;

    public void CallRegister()
    {
        CmdCallRegister(clawmailField.text, hash, salt, userType.transform.GetChild(0).GetComponent<Text>().text);
    }

    [Command]
    public void CmdCallRegister(string clawmailField, string hash, string salt, string userType)
    {
        if (isServer)
            Debug.Log("register submit pressed");
        StartCoroutine(Register(connectionToClient, clawmailField, hash, salt, userType));
    }

    IEnumerator Register(NetworkConnection target, string clawmailField, string hash, string salt, string userType)
    {
        Debug.Log("register (www) coroutine started on server");
        WWWForm form = new WWWForm();
        form.AddField("clawmail", clawmailField);
        form.AddField("hash", hash);
        form.AddField("salt", salt);
        form.AddField("userType", userType);
        WWW www = new WWW("http://localhost/register.php", form);
        yield return www;
        if (www.text == "0") //no errors
        {
            string update = "User created successfully.";
            Debug.Log(update);
            TargetShowRegistrationUpdate(target, update);
        }
        else
        {
            string update = "User creation failed. Error: " + www.text;
            Debug.Log(update);
            TargetShowRegistrationUpdate(target, update);
        }
    }

    [TargetRpc]
    public void TargetShowRegistrationUpdate(NetworkConnection target, string update)
    {
        StartCoroutine(ShowRegistrationUpdate(update));
    }

    IEnumerator ShowRegistrationUpdate(string update)
    {
        if (update.Contains("success"))
            mmc.ClearInput();
        GameObject.Find("ErrorMsg").GetComponent<Text>().text = update;
        yield return new WaitForSeconds(10f);
        GameObject.Find("ErrorMsg").GetComponent<Text>().text = msgHint;
    }

    public void VerifyInputs()
    {
        print("verifying inputs");

        if (GameObject.Find("MainMenu").GetComponent<MainMenuController>().registerPanel.activeSelf)
        {
            if (clawmailField.text.Contains("@ggc.edu"))
                clawmailField.text.Remove((clawmailField.text.IndexOf('@') + 7));

            if (clawmailField.text != "" && passwordField.text != "")
            {
                //byte[] password = ASCIIEncoding.ASCII.GetBytes("" + passwordField.text);
                /*SHA512 sha = new SHA512Managed();
                byte[] saltByte = sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes("ourlordandsaviorkirby" + clawmailField.text)); //possibly could work and lower character count? haven't tried yet
                                                                                                                               //byte[] saltByte = sha.Hash;
                StringBuilder strBuilder = new StringBuilder();
                for (int i = 0; i < saltByte.Length; i++)
                {
                    strBuilder.Append(saltByte[i].ToString("x2"));
                }
                salt = strBuilder.ToString();
                //salt = salt.Remove(50);
                //Debug.Log("salt: " + salt);*/

                /*CrypterOptionKey key = new CrypterOptionKey("A key used to customize the salt based on the username", clawmailField.text.GetType());
                CrypterOptions option = new CrypterOptions();
                option.Add(key, clawmailField.text);
                salt = Crypter.Blowfish.GenerateSalt(option);*/
                //salt = clawmailField.text + "ourlordandsaviorkirby";
                //salt = Crypter.Sha512.GenerateSalt();
                salt = Crypter.Blowfish.GenerateSalt();
                //print("cancrypt: " + Crypter.Blowfish.CanCrypt(salt));
                //salt = Crypter.Blowfish.Crypt(salt);
                Debug.Log("salt: " + salt);
                //hash = passwordField.text + salt;
                hash = Crypter.Blowfish.Crypt(passwordField.text, salt); //hash = UnixCrypt.Crypt(salt, passwordField.text); //HashAlgorithm.Create();
                Debug.Log("pswd: " + hash);
                Debug.Log("registration rehash: " + Crypter.Blowfish.Crypt(passwordField.text, salt));
            }

            registerBtn.interactable = (clawmailField.text.Contains("@ggc.edu") && !clawmailField.text.Substring(0, 1).Equals("@") && passwordField.text.Length >= 8 && userType.value != 0);
        }
        else if (GameObject.Find("MainMenu").GetComponent<MainMenuController>().loginPanel.activeSelf)
        {
            if (clawmailFieldLogin.text.Contains("@ggc.edu"))
                clawmailFieldLogin.text.Remove((clawmailFieldLogin.text.IndexOf('@') + 7));

            loginBtn.interactable = (clawmailFieldLogin.text.Contains("@ggc.edu") && !clawmailFieldLogin.text.Substring(0, 1).Equals("@") && passwordFieldLogin .text.Length >= 1);
        }
    }
    #endregion

    #region login
    [SerializeField] InputField clawmailFieldLogin;
    [SerializeField] InputField passwordFieldLogin;
    [SerializeField] Button loginBtn;

    public void CallLogin()
    {
        CmdCallLogin(clawmailFieldLogin.text);
    }

    [Command]
    public void CmdCallLogin(string clawmailFieldLogin)
    {
        if (isServer)
            Debug.Log("login submit pressed");
        StartCoroutine(Login(connectionToClient, clawmailFieldLogin));
    }

    IEnumerator Login(NetworkConnection target, string clawmailFieldLogin)
    {
        Debug.Log("login (www) coroutine started on server");
        WWWForm form = new WWWForm();
        form.AddField("clawmail", clawmailFieldLogin);
        WWW www = new WWW("http://localhost/login.php", form);
        yield return www;
        /*if (www.text == "0") //no errors
        {
            string update = "User logged in successfully.";
            Debug.Log(update);
            RpcShowLoginUpdate(update);
        }
        else
        {
            string update = "User log in failed. Error: " + www.text;
            Debug.Log(update);
            RpcShowLoginUpdate(update);
        }*/
        if (www.text.Contains("0"))
        {
            string[] results = www.text.Split('ツ');
            string usersalt = results[1];
            string userhash = results[2];
            string userlegal = results[3];
            string isBanned = results[4];
            TargetSendSaltToClient(target, usersalt, userhash, userlegal, isBanned);
        }
    }

    [TargetRpc]
    public void TargetSendSaltToClient(NetworkConnection target, string usersalt, string userhash, string userlegal, string isBanned)
    {
        print("received usersalt: " + usersalt + ", received userhash: " + userhash);
        Debug.Log("client checking login credentials");
        if (Crypter.Blowfish.Crypt(passwordFieldLogin.text, usersalt) == userhash)
        {
            Debug.Log("correct credentials");
            clawmail = clawmailFieldLogin.text;
            mmc.ClearInput();
            if (isBanned.Contains("yes"))
            {
                StartCoroutine(ShowLoginUpdate("Account banned for bad vibes.", false, ""));
            }
            else
                StartCoroutine(ShowLoginUpdate("Thanks for logging in, " + clawmail + "! Opening main menu...", true, userlegal));
        }
        else
            StartCoroutine(ShowLoginUpdate("Credentials mismatch :(", false, ""));
        Debug.Log("rehash: " + Crypter.Blowfish.Crypt(passwordField.text, usersalt) + ", userhash: " + userhash);
    }

    IEnumerator ShowLoginUpdate(string update, bool check, string userlegal)
    {
        if (check)
        {
            GameObject.Find("ErrorMsg").GetComponent<Text>().text = update;
            yield return new WaitForSeconds(3f);
            GameObject.Find("ErrorMsg").GetComponent<Text>().text = msgHint;
            //deactivate login/register panel and show main menu stuff
            mmc.CloseLoginRegisterPanels();
            mmc.OpenMainMenuOptions(clawmail);
            Debug.Log("userlegal: " + userlegal);
            if (userlegal == "no")
                mmc.OpenLegalAgreementPanel();
        }
        else
        {
            GameObject.Find("ErrorMsg").GetComponent<Text>().text = update;
            yield return new WaitForSeconds(10f);
            if (GameObject.Find("ErrorMsg") != null)
                GameObject.Find("ErrorMsg").GetComponent<Text>().text = msgHint;
        }
    }
    #endregion

    #region view public topic vibes
    Text test;
    [SerializeField] Button refreshPublicTopicVibesBtn;
    List<string[]> results;

    public void RefreshPublicTopicVibes()
    {
        CmdRefreshPublicTopicVibes(clawmail);
    }

    [Command]
    void CmdRefreshPublicTopicVibes(string clawmail)
    {
        StartCoroutine(GetPublicTopicVibes(connectionToClient, clawmail));
    }

    IEnumerator GetPublicTopicVibes(NetworkConnection target, string clawmail)
    {
        Debug.Log("get public topic vibes (www) coroutine started on server");
        WWWForm form = new WWWForm();
        form.AddField("clawmail", clawmail);
        WWW www = new WWW("http://localhost/publictopicvibes.php", form);
        yield return www;
        if (www.text != null && www.text != "")
        {
            TargetSendPublicVibesToClient(target, www.text);
        }
        else
            TargetSendPublicVibesToClientFail(target, "no results");
    }

    [TargetRpc]
    public void TargetSendPublicVibesToClient(NetworkConnection target, string webresult)
    {
        Debug.Log("webresult/www.text: " + webresult);
        results = new List<string[]>();
        string[] text = webresult.Split('ツ');
        foreach (string res in text)
        {
            string[] ree = res.Split(':');
            results.Add(ree);
        }
        results.RemoveAt(results.Count - 1);
        //test.text = "";
        int topicIndex = 0;
        foreach (string[] data in results)
        {
            Debug.Log("public data length: " + data.Length);
            for (int i = 0; i < 1 && topicIndex < 10; i++)
            {
                topics[topicIndex].transform.GetChild(0).GetComponent<Text>().text = data[i];
                topicIndex++;
            }
        }
    }

    [TargetRpc]
    public void TargetSendPublicVibesToClientFail(NetworkConnection target, string results)
    {
        test.text = "no results";
    }
    #endregion

    #region reply to topic vibe
    private Text topic;
    private Text message;
    private InputField inputField;
    private GameObject inputs;
    private GameObject scrollView;
    private Button postReplyMsg;
    private string button;
    private GameObject replyCanvas;
    private GameObject messagesCanvas;
    private GameObject playerPrefab;
    private GameObject getPosts;
    private GameObject updateMsg;
    private GameObject[] topics;
    
    int msgID;

    public void postReply()
    {
        if (wf.checkWords(inputField))
        {
            CmdPostReply(inputField.text, clawmail, msgID);
        }
        else
        {
            StartCoroutine(ShowUpdateTest("not very cash munny of u"));
        }
    }

    [Command]
    public void CmdPostReply(string replyField, string clawmail, int messageID)
    {
        if (isServer)
            Debug.Log("post reply has been pressed");
        StartCoroutine(Reply(connectionToClient, replyField, clawmail, messageID));
    }

    IEnumerator Reply(NetworkConnection target, string replyField, string clawmail, int messageID)
    {
        Debug.Log("post reply to topic vibe (www) coroutine started on server");
        WWWForm replyForm = new WWWForm();
        replyForm.AddField("reply", replyField);
        replyForm.AddField("clawmail", clawmail);
        replyForm.AddField("messageID", messageID);
        WWW www = new WWW("http://localhost/sendReply.php", replyForm);
        yield return www;

        if (www.text == "0") //no errors
        {
            string update = "reply added successfully.";
            Debug.Log(update);
            TargetShowUpdateTest(target, update);
        }
        else
        {
            string update = "reply not added: " + www.text;
            Debug.Log(update);
            TargetShowUpdateTest(target, update);
        }
        //Debug.Log("Server connected to DB");
    }

    [TargetRpc]
    void TargetShowUpdateTest(NetworkConnection target, string update)
    {
        StartCoroutine(ShowUpdateTest(update));
    }

    IEnumerator ShowUpdateTest(string update)
    {
        if (update.Contains("success"))
            mmc.ClearInput();
        updateMsg.GetComponent<Text>().text = "" + update;
        yield return new WaitForSeconds(3);
        updateMsg.GetComponent<Text>().text = "";
    }

    public void topicClicked()
    {
        print("topic clicked");
        refreshPublicTopicVibesBtn.gameObject.SetActive(false);
        //replyCanvas = GameObject.Find("ReplyCanvas");
        replyCanvas.SetActive(true);
        topic = GameObject.Find("subject").GetComponent<Text>();
        message = GameObject.Find("message").GetComponent<Text>();
        scrollView = GameObject.Find("Scroll View");
        button = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(button);
        Debug.Log("tc" + scrollView.activeInHierarchy);
        string txt = GameObject.Find(button).GetComponentInChildren<Text>().text;
        topic.text = txt;

        foreach (string[] data in results)
        {
            if (data[0] == topic.text)
            {
                message.text = data[1];
                msgID = int.Parse(data[2]);
            }
        }
        scrollView.SetActive(false);
    }

    void GetPosts()
    {
        print("go back to posts");
        refreshPublicTopicVibesBtn.gameObject.SetActive(true);
        replyCanvas.SetActive(false);
        scrollView.SetActive(true);
    }

    public void displayMessagesCanvas()
    {
        //messagesCanvas = GameObject.Find("TopicsCanvas");
        messagesCanvas.SetActive(true);
    }
    #endregion

    #region view personal topic vibes
    List<string[]> personalResults;
    GameObject personalTopics;
    GameObject personalTopicVibesRepliesPanel;
    public GameObject PersonalTopicBtn;
    public GameObject PersonalReplyMsg;
    List<string[]> topicVibeReplyResults;
    GameObject personalTopicVibesRepliesScrollview;
    Text personalsubject;
    Text personalmessage;
    int idVibe;

    public void RefreshPersonalTopicVibes()
    {
        CmdRefreshPersonalTopicVibes(clawmail);
        foreach (Transform msg in personalTopics.transform.GetComponent<Transform>())
        {
            Destroy(msg.gameObject);
        }
    }

    [Command]
    void CmdRefreshPersonalTopicVibes(string clawmail)
    {
        StartCoroutine(GetPersonalTopicVibes(connectionToClient, clawmail));
    }

    IEnumerator GetPersonalTopicVibes(NetworkConnection target, string clawmail)
    {
        Debug.Log("get personal topic vibes (www) coroutine started on server");
        WWWForm form = new WWWForm();
        form.AddField("clawmail", clawmail);
        WWW www = new WWW("http://localhost/personaltopicvibes.php", form);
        yield return www;
        if (www.text != null && www.text != "")
        {
            TargetSendPersonalVibesToClient(target, www.text);
        }
        else
            TargetSendPersonalVibesToClientFail(target, "no results");
    }

    [TargetRpc]
    public void TargetSendPersonalVibesToClient(NetworkConnection target, string webresult)
    {
        Debug.Log("webresult/www.text: " + webresult);
        personalResults = new List<string[]>();
        string[] text = webresult.Split('ツ');
        foreach (string res in text)
        {
            Debug.Log("res: " + res);

            string[] ree = res.Split(':');
            foreach (string str in ree)
            {
                Debug.Log("str: " + str);
            }
            personalResults.Add(ree);
        }
        personalResults.RemoveAt(personalResults.Count - 1);
        //test.text = "";
        //int topicIndex = 0;
        foreach (string[] data in personalResults)
        {
            Debug.Log("data length: " + data.Length);
            foreach (string s in data)
            {
                Debug.Log("field in row of data: " + s);
            }
            GameObject btn = Instantiate(PersonalTopicBtn, personalTopics.transform);
            btn.transform.GetChild(0).GetComponent<Text>().text = data[1]; //subject
            Debug.Log(data[2]);
            btn.GetComponent<Button>().onClick.AddListener(PersonalTopicClicked);
            //topicIndex++;
        }
    }

    [TargetRpc]
    public void TargetSendPersonalVibesToClientFail(NetworkConnection target, string results)
    {
        test.text = results;
    }

    void PersonalTopicClicked()
    {
        personalTopicVibesRepliesPanel.SetActive(true);
        string subject = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
        idVibe = 0;
        personalsubject = GameObject.Find("personalsubject").GetComponent<Text>();
        personalmessage = GameObject.Find("personalmessage").GetComponent<Text>();
        foreach (string[] data in personalResults)
        {
            if (data[1] == subject)
            {
                personalsubject.text = subject;
                personalmessage.text = data[2];
                idVibe = int.Parse(data[4]);
                msgID = int.Parse(data[4]);
            }
        }
        CmdGetPersonalTopicVibeReplies(idVibe);
    }

    void ClosePersonalTopicVibesRepliesPanel()
    {
        personalTopicVibesRepliesPanel.SetActive(false);
        foreach (Transform msg in personalTopicVibesRepliesScrollview.transform.GetComponent<Transform>())
        {
            Destroy(msg.gameObject);
        }
    }

    [Command]
    void CmdGetPersonalTopicVibeReplies(int idVibe)
    {
        StartCoroutine(GetPersonalTopicVibeReplies(connectionToClient, idVibe));
    }

    IEnumerator GetPersonalTopicVibeReplies(NetworkConnection target, int idVibe)
    {
        Debug.Log("get personal topic vibe replies (www) coroutine started on server");
        WWWForm form = new WWWForm();
        form.AddField("idVibe", idVibe);
        WWW www = new WWW("http://localhost/personaltopicvibereplies.php", form);
        yield return www;
        if (www.text != null && www.text != "")
        {
            TargetSendPersonalTopicVibeRepliesToClient(target, www.text);
        }
        else
            TargetSendPersonalTopicVibeRepliesToClientFail(target, "no results");
    }

    [TargetRpc]
    public void TargetSendPersonalTopicVibeRepliesToClient(NetworkConnection target, string webresult)
    {
        Debug.Log("webresult/www.text: " + webresult);
        topicVibeReplyResults = new List<string[]>();
        string[] text = webresult.Split('ツ');
        foreach (string res in text)
        {
            Debug.Log("res: " + res);

            string[] ree = res.Split(':');
            foreach (string str in ree)
            {
                Debug.Log("str: " + str);
            }
            topicVibeReplyResults.Add(ree);
        }
        topicVibeReplyResults.RemoveAt(topicVibeReplyResults.Count - 1);
        foreach (string[] data in topicVibeReplyResults)
        {
            Debug.Log("data length: " + data.Length);
            foreach (string s in data)
            {
                Debug.Log("field in row of data: " + s);
            }
            if (PersonalReplyMsg == null)
                Debug.Log("personal reply msg prefab is null");
            else if (personalTopicVibesRepliesScrollview == null)
            {
                Debug.Log("scrollview is null");
                personalTopicVibesRepliesScrollview = GameObject.Find("PersonalTopicVibesRepliesScrollview");
            }
            if (personalTopicVibesRepliesScrollview == null)
                Debug.Log("scrollview is still null");
            GameObject txt = Instantiate(PersonalReplyMsg, personalTopicVibesRepliesScrollview.transform);
            txt.GetComponent<Text>().text = data[0] + "\n- " + data[1]; //subject
            txt.GetComponent<Button>().onClick.AddListener(showReport);
            txt.GetComponent<Button>().onClick.AddListener(SetMsgID);
        }
    }

    void SetMsgID()
    {
        print("current obj: " + EventSystem.current.currentSelectedGameObject.name);
        foreach (string[] data in topicVibeReplyResults)
        {
            if (EventSystem.current.currentSelectedGameObject.GetComponent<Text>().text.Contains(data[0]))
            {
                msgID = int.Parse(data[2]);
                print("msgID: " + msgID);
            }
        }
    }

    [TargetRpc]
    public void TargetSendPersonalTopicVibeRepliesToClientFail(NetworkConnection target, string results)
    {
        Debug.Log(results);
    }
    #endregion

    #region send topic vibe
    private GameObject topicCanvas;
    private InputField topicInputField;
    private InputField subjectInputField;
    private GameObject topicUpdateMsg;
    private Button postTopicMsg;

    public void postTopic()
    {
        if (wf.checkWords(topicInputField) && wf.checkWords(subjectInputField))
        {
            CmdPostTopic(topicInputField.text, subjectInputField.text, clawmail);
            mmc.TogglePostTopicVibesPanel();
        }
        else
        {
            StartCoroutine(PostTopicUpdate("not very cash munny of u"));
        }

    }

    [Command]
    public void CmdPostTopic(string topicField, string subjectField, string clawmail)
    {
        if (isServer)
            Debug.Log("post topic vibe has been pressed");
        StartCoroutine(Topic(connectionToClient, topicField, subjectField, clawmail));
    }

    IEnumerator Topic(NetworkConnection target, string topicField, string subjectField, string clawmail)
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
            string update = "topic vibe added successfully.";
            Debug.Log(update);
            TargetShowTopicUpdateTest(target, update);
        }
        //Debug.Log("Server connected to DB");
    }

    [TargetRpc]
    void TargetShowTopicUpdateTest(NetworkConnection target, string update)
    {
        StartCoroutine(PostTopicUpdate(update));
    }

    IEnumerator PostTopicUpdate(string update)
    {
        mmc.ClearInput();
        topicUpdateMsg.GetComponent<Text>().text = "" + update;
        yield return new WaitForSeconds(3);
        topicUpdateMsg.GetComponent<Text>().text = "";
    }
    #endregion

    #region legal agreement
    public void Agree()
    {
        CmdAgree(clawmail);
    }

    [Command]
    void CmdAgree(string clawmail)
    {
        if (isServer)
            Debug.Log("agree to legal stuff has been pressed");
        StartCoroutine(AgreeToLegal(connectionToClient, clawmail));
    }

    IEnumerator AgreeToLegal(NetworkConnection target, string clawmail)
    {
        Debug.Log("agree to legal (www) coroutine started on server");
        WWWForm legalForm = new WWWForm();
        legalForm.AddField("clawmail", clawmail);
        WWW www = new WWW("http://localhost/legal.php", legalForm);
        yield return www;

        if (www.text == "0") //no errors
        {
            string update = "agreement status updated.";
            Debug.Log(update);
        }
    }
    #endregion

    #region ManualReport

    public GameObject reportCanvas;
    private Button sendReportBtn;
    private Button reporting;
    private Button closeReportBtn;
    public Dropdown reportCategory;
    private int openCanvas; //canvases
    private GameObject personalTopicVibesPanel;

    public void sendReport()
    {
        CmdReport(clawmail, msgID, reportCategory.transform.GetChild(0).GetComponent<Text>().text);
    }

    [Command]
    public void CmdReport(string clawmail, int messageID, string reportCategory)
    {
        if (isServer)
            Debug.Log("send report has been pressed");
        StartCoroutine(Report(connectionToClient, clawmail, messageID, reportCategory));
    }

    IEnumerator Report(NetworkConnection target, string clawmail, int messageID, string reportCategory)
    {
        Debug.Log("report topic or reply vibe (www) coroutine started on server");
        WWWForm reportForm = new WWWForm();
        reportForm.AddField("clawmail", clawmail);
        reportForm.AddField("messageID", messageID);
        reportForm.AddField("reportCategory", reportCategory);
        WWW www = new WWW("http://localhost/report.php", reportForm);
        yield return www;
        TargetShowReportUpdate(target, www.text);
    }

   [TargetRpc]
   void TargetShowReportUpdate(NetworkConnection target, string result)
    {
        StartCoroutine(ShowReportUpdate(result));
    }

    IEnumerator ShowReportUpdate(string result)
    {
        print("report results: " + result);
        print("openCanvas: " + openCanvas);
        if (result.Contains("0"))
        {
            hideReport();
            if (openCanvas == 3)
            {
                //ClosePersonalTopicVibesRepliesPanel();
                foreach (Transform msg in personalTopicVibesRepliesScrollview.transform.GetComponent<Transform>())
                {
                    Destroy(msg.gameObject);
                }
                CmdGetPersonalTopicVibeReplies(idVibe);
            }
            updateMsg.GetComponent<Text>().text = "Vibe reported successfully.";
            yield return new WaitForSeconds(3);
            updateMsg.GetComponent<Text>().text = "";
        }
        else
        {
            updateMsg.GetComponent<Text>().text = "Error reporting vibe.";
            yield return new WaitForSeconds(3);
            updateMsg.GetComponent<Text>().text = "";
        }
    }

    public void hideReport()
    {
        reportCategory.value = 0;
        reportCanvas.SetActive(false);
        if (openCanvas == 1)
            replyCanvas.SetActive(true);
        else if (openCanvas == 2)
            GameObject.Find("PostTopicVibePanel").SetActive(true);
        else if (openCanvas == 3)
            personalTopicVibesPanel.SetActive(true);

    }

    public void showReport()
    {
        reportCanvas.SetActive(true);
        personalTopicVibesPanel = GameObject.Find("PersonalTopicVibesRepliesPanel");
        if (GameObject.Find("PublicTopicVibesPanel") != null && GameObject.Find("PublicTopicVibesPanel").activeSelf)
        {
            openCanvas = 1;
            replyCanvas.SetActive(false);
        }
        else if (GameObject.Find("PostTopicVibePanel") != null && GameObject.Find("PostTopicVibePanel").activeSelf)
        {
            openCanvas = 2;
            GameObject.Find("PostTopicVibePanel").SetActive(false);
        }
        else if (personalTopicVibesPanel != null && GameObject.Find("PersonalTopicVibesRepliesPanel").activeSelf)
        {
            openCanvas = 3;
            personalTopicVibesPanel.SetActive(false);
            print("personal replies panel disappeared");
        }

    }
    #endregion

    private void Awake()
    {
        print("player instantiated in " + SceneManager.GetActiveScene().name);

        timeLeft = 3;
        print("timeLeft set");
        DontDestroyOnLoad(this.gameObject);
    }

    #region demo stuff 2
    public override void OnStartServer()
    {
        netMan = GameObject.Find("NetMan").GetComponent<NetworkManager>();
        db = netMan.GetComponent<PostDB>();
    }
    #endregion

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
            CmdAskLoadMainMenuScene();
        }

        if (SceneManager.GetActiveScene().name == "MainMenu") //doesn't get called
        {
            Screen.orientation = ScreenOrientation.Portrait;
            gameObject.name = "LocalPlayer";
        }
    }
    private void Update()
    {
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            mmc = GameObject.Find("MainMenu").GetComponent<MainMenuController>();
            mmc.SetLocalPlayer(this);

            msgHint = GameObject.Find("ErrorMsg").GetComponent<Text>().text;

            #region register panel 
            clawmailField = GameObject.Find("RegisterClawmailField").GetComponent<InputField>();
            /*EventTrigger eventTrigger = clawmailField.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Select,
                callback = new EventTrigger.TriggerEvent()
            };
            UnityEngine.Events.UnityAction<BaseEventData> callback =
            new UnityEngine.Events.UnityAction<BaseEventData>(SelectEventMethod);
            entry.callback.AddListener(callback);
            eventTrigger.triggers.Add(entry);*/

            passwordField = GameObject.Find("RegisterPasswordField").GetComponent<InputField>();
            /*EventTrigger eventTrigger2 = passwordField.GetComponent<EventTrigger>();
            EventTrigger.Entry entry2 = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Select,
                callback = new EventTrigger.TriggerEvent()
            };
            UnityEngine.Events.UnityAction<BaseEventData> callback2 =
            new UnityEngine.Events.UnityAction<BaseEventData>(SelectEventMethod);
            entry2.callback.AddListener(callback2);
            eventTrigger2.triggers.Add(entry2);*/

            userType = GameObject.Find("RegisterUserType").GetComponent<Dropdown>();

            registerBtn = GameObject.Find("SubmitRegister").GetComponent<Button>();
            registerBtn.onClick.AddListener(CallRegister);
            #endregion

            #region login panel
            clawmailFieldLogin = GameObject.Find("LoginClawmailField").GetComponent<InputField>();
            /*EventTrigger eventTrigger3 = clawmailFieldLogin.GetComponent<EventTrigger>();
            EventTrigger.Entry entry3 = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Select,
                callback = new EventTrigger.TriggerEvent()
            };
            UnityEngine.Events.UnityAction<BaseEventData> callback3 =
            new UnityEngine.Events.UnityAction<BaseEventData>(SelectEventMethod);
            entry3.callback.AddListener(callback3);
            eventTrigger3.triggers.Add(entry3);*/

            passwordFieldLogin = GameObject.Find("LoginPasswordField").GetComponent<InputField>();
            /*EventTrigger eventTrigger4 = passwordFieldLogin.GetComponent<EventTrigger>();
            EventTrigger.Entry entry4 = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Select,
                callback = new EventTrigger.TriggerEvent()
            };
            UnityEngine.Events.UnityAction<BaseEventData> callback4 =
            new UnityEngine.Events.UnityAction<BaseEventData>(SelectEventMethod);
            entry4.callback.AddListener(callback4);
            eventTrigger4.triggers.Add(entry4);*/

            loginBtn = GameObject.Find("SubmitLogin").GetComponent<Button>();
            loginBtn.onClick.AddListener(CallLogin);
            #endregion
            
            #region public topic vibes panel
            test = mmc.testText.GetComponent<Text>();
            refreshPublicTopicVibesBtn = GameObject.Find("PublicTopicVibesRefreshBtn").GetComponent<Button>();
            refreshPublicTopicVibesBtn.onClick.AddListener(RefreshPublicTopicVibes);
            refreshPublicTopicVibesBtn.onClick.AddListener(displayMessagesCanvas);
            #endregion

            #region reply to public topic vibes panel
            replyCanvas = GameObject.Find("ReplyCanvas");
            messagesCanvas = GameObject.Find("TopicsCanvas");
            postReplyMsg = GameObject.Find("PostMsg").GetComponent<Button>();
            inputs = GameObject.Find("reply");
            topic = GameObject.Find("subject").GetComponent<Text>();
            message = GameObject.Find("message").GetComponent<Text>();
            inputField = GameObject.Find("ReplyInputField").GetComponent<InputField>();
            scrollView = GameObject.Find("Scroll View");
            updateMsg = GameObject.Find("UpdateMsg");
            topics = GameObject.FindGameObjectsWithTag("Topics");

            //inputs.SetActive(false);
            

            postReplyMsg.onClick.AddListener(postReply);
            postReplyMsg.onClick.AddListener(GetPosts);
            foreach (GameObject button in topics)
            {
                button.GetComponent<Button>().onClick.AddListener(topicClicked);
            }

            reporting = GameObject.Find("Reportbtn").GetComponent<Button>();
            reporting.onClick.AddListener(showReport);
            #endregion

            #region personal topic vibes panel
            personalTopics = GameObject.Find("PersonalTopics");
            mmc.personalTopicVibesBtn.GetComponent<Button>().onClick.AddListener(RefreshPersonalTopicVibes);
            personalTopicVibesRepliesPanel = GameObject.Find("PersonalTopicVibesRepliesPanel");
            personalTopicVibesRepliesPanel.GetComponent<Button>().onClick.AddListener(ClosePersonalTopicVibesRepliesPanel);
            personalTopicVibesRepliesPanel.SetActive(false);
            personalTopicVibesRepliesScrollview = GameObject.Find("PersonalTopicVibesRepliesScrollview");
            getPosts = GameObject.Find("GetPosts");
            getPosts.GetComponent<Button>().onClick.AddListener(GetPosts);
            #endregion

            #region send topic vibes panel
            topicCanvas = GameObject.Find("SendTopicCanvas");
            topicInputField = GameObject.Find("SendTopicMessageInputField").GetComponent<InputField>();
            subjectInputField = GameObject.Find("SendTopicSubjectInputField").GetComponent<InputField>();
            topicUpdateMsg = GameObject.Find("SendTopicUpdateMsg");
            postTopicMsg = GameObject.Find("PostTopicMsg").GetComponent<Button>();

            postTopicMsg.GetComponent<Button>().onClick.AddListener(postTopic);

            //from replies
            messagesCanvas.SetActive(false);
            replyCanvas.SetActive(false);
            #endregion

            #region reporting
            reportCanvas = GameObject.Find("ReportCanvas");
            sendReportBtn = GameObject.Find("SendReport").GetComponent<Button>();
            reportCategory = GameObject.Find("ReportCategory").GetComponent<Dropdown>();
            closeReportBtn = GameObject.Find("CloseReport").GetComponent<Button>();
            personalTopicVibesPanel = GameObject.Find("PersonalTopicVibesRepliesPanel");

            sendReportBtn.onClick.AddListener(sendReport);
            closeReportBtn.onClick.AddListener(hideReport);
            reportCategory.value = 0;
            reportCanvas.SetActive(false);
            #endregion
        }
    }

    #region client startup
    [Command]
    public void CmdAskLoadConnectingScene() //test to see if messages get passed between server<-->client and scene changes on server
    {
        TargetClientLoadConnectingScene(connectionToClient);
    }

    [Command]
    public void CmdAskLoadMainMenuScene()
    {
        TargetClientLoadMainMenuScene(connectionToClient);
    }

    [TargetRpc]
    public void TargetClientLoadConnectingScene(NetworkConnection target)
    {
        SceneManager.LoadScene("Connecting");
    }

    [TargetRpc]
    public void TargetClientLoadMainMenuScene(NetworkConnection target)
    {
        StartCoroutine(ClientConnected());
    }

    IEnumerator ClientConnected()
    {
        if (SceneManager.GetActiveScene().name == "Connecting")
        {
            print("timeLeft: " + timeLeft);
            connecting.text = "Connected! Login screen will show up in " + timeLeft + " seconds";
            yield return new WaitForSeconds(1);
            timeLeft--;
            if (timeLeft == 0)
            {
                SceneManager.LoadScene("MainMenu");
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
                StartCoroutine(ClientConnected());
        }
    }
    #endregion

    #region demo
    /*[Command]
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
    }*/
    #endregion
}