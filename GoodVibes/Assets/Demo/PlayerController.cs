using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
//using DigiWar.Security.Cryptography;
using CryptSharp;
using CryptSharp.Utility;
using System.Text;
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
        StartCoroutine(Register(clawmailField, hash, salt, userType));
    }

    IEnumerator Register(string clawmailField, string hash, string salt, string userType)
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
            RpcShowRegistrationUpdate(update);
        }
        else
        {
            string update = "User creation failed. Error: " + www.text;
            Debug.Log(update);
            RpcShowRegistrationUpdate(update);
        }
    }

    [ClientRpc]
    public void RpcShowRegistrationUpdate(string update)
    {
        StartCoroutine(ShowRegistrationUpdate(update));
    }

    IEnumerator ShowRegistrationUpdate(string update)
    {
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

            loginBtn.interactable = (clawmailFieldLogin.text.Contains("@ggc.edu") && !clawmailFieldLogin.text.Substring(0, 1).Equals("@"));
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
        StartCoroutine(Login(clawmailFieldLogin));
    }

    IEnumerator Login(string clawmailFieldLogin)
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
            string[] results = www.text.Split('/');
            string usersalt = results[1];
            string userhash = results[2];
            RpcSendSaltToClient(usersalt, userhash);
        }
    }

    [ClientRpc]
    public void RpcSendSaltToClient(string usersalt, string userhash)
    {
        print("received usersalt: " + usersalt + ", received userhash: " + userhash);
        Debug.Log("client checking login credentials");
        if (Crypter.Blowfish.Crypt(passwordFieldLogin.text, usersalt) == userhash)
        {
            Debug.Log("correct credentials");
            clawmail = clawmailFieldLogin.text;
            StartCoroutine(ShowLoginUpdate("Thanks for logging in, " + clawmail + "! Opening main menu...", true));
        }
        else
            StartCoroutine(ShowLoginUpdate("Credentials mismatch :(", false));
        Debug.Log("rehash: " + Crypter.Blowfish.Crypt(passwordField.text, usersalt) + ", userhash: " + userhash);
    }

    IEnumerator ShowLoginUpdate(string update, bool check)
    {
        if (check)
        {
            GameObject.Find("ErrorMsg").GetComponent<Text>().text = update;
            yield return new WaitForSeconds(3f);
            GameObject.Find("ErrorMsg").GetComponent<Text>().text = msgHint;
            //deactivate login/register panel and show main menu stuff
            mmc.CloseLoginRegisterPanels();
            mmc.OpenMainMenuOptions(clawmail);
        }
        else
        {
            GameObject.Find("ErrorMsg").GetComponent<Text>().text = update;
            yield return new WaitForSeconds(10f);
            GameObject.Find("ErrorMsg").GetComponent<Text>().text = msgHint;
        }
    }
    #endregion

    #region view public topic vibes
    Text test;
    [SerializeField] Button refreshPublicTopicVibesBtn;

    public void RefreshPublicTopicVibes()
    {
        CmdRefreshPublicTopicVibes();
    }

    [Command]
    void CmdRefreshPublicTopicVibes()
    {
        StartCoroutine(GetPublicTopicVibes());
    }

    IEnumerator GetPublicTopicVibes()
    {
        Debug.Log("get public topic vibes (www) coroutine started on server");
        WWW www = new WWW("http://localhost/publictopicvibes.php");
        yield return www;
        if (www.text != null && www.text != "")
        {
            string[] results = www.text.Split('/');
            RpcSendPublicVibesToClient(results);
        }
        else
            RpcSendPublicVibesToClient(new string[] {"no results"});
    }

    [ClientRpc]
    public void RpcSendPublicVibesToClient(string[] results)
    {
        foreach (string result in results)
        {
            test.text += result;
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

            msgHint = GameObject.Find("ErrorMsg").GetComponent<Text>().text;

            #region register panel 
            clawmailField = GameObject.Find("RegisterClawmailField").GetComponent<InputField>();
            EventTrigger eventTrigger = clawmailField.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Select,
                callback = new EventTrigger.TriggerEvent()
            };
            UnityEngine.Events.UnityAction<BaseEventData> callback =
            new UnityEngine.Events.UnityAction<BaseEventData>(SelectEventMethod);
            entry.callback.AddListener(callback);
            eventTrigger.triggers.Add(entry);

            passwordField = GameObject.Find("RegisterPasswordField").GetComponent<InputField>();
            EventTrigger eventTrigger2 = passwordField.GetComponent<EventTrigger>();
            EventTrigger.Entry entry2 = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Select,
                callback = new EventTrigger.TriggerEvent()
            };
            UnityEngine.Events.UnityAction<BaseEventData> callback2 =
            new UnityEngine.Events.UnityAction<BaseEventData>(SelectEventMethod);
            entry2.callback.AddListener(callback2);
            eventTrigger2.triggers.Add(entry2);

            userType = GameObject.Find("RegisterUserType").GetComponent<Dropdown>();

            registerBtn = GameObject.Find("SubmitRegister").GetComponent<Button>();
            registerBtn.onClick.AddListener(CallRegister);
            #endregion

            #region login panel
            clawmailFieldLogin = GameObject.Find("LoginClawmailField").GetComponent<InputField>();
            EventTrigger eventTrigger3 = clawmailFieldLogin.GetComponent<EventTrigger>();
            EventTrigger.Entry entry3 = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Select,
                callback = new EventTrigger.TriggerEvent()
            };
            UnityEngine.Events.UnityAction<BaseEventData> callback3 =
            new UnityEngine.Events.UnityAction<BaseEventData>(SelectEventMethod);
            entry3.callback.AddListener(callback3);
            eventTrigger3.triggers.Add(entry3);

            passwordFieldLogin = GameObject.Find("LoginPasswordField").GetComponent<InputField>();
            EventTrigger eventTrigger4 = passwordFieldLogin.GetComponent<EventTrigger>();
            EventTrigger.Entry entry4 = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Select,
                callback = new EventTrigger.TriggerEvent()
            };
            UnityEngine.Events.UnityAction<BaseEventData> callback4 =
            new UnityEngine.Events.UnityAction<BaseEventData>(SelectEventMethod);
            entry4.callback.AddListener(callback4);
            eventTrigger4.triggers.Add(entry4);

            loginBtn = GameObject.Find("SubmitLogin").GetComponent<Button>();
            loginBtn.onClick.AddListener(CallLogin);
            #endregion

            #region public topic vibes panel
            test = mmc.testText.GetComponent<Text>();
            refreshPublicTopicVibesBtn = GameObject.Find("PublicTopicVibesRefreshBtn").GetComponent<Button>();
            refreshPublicTopicVibesBtn.onClick.AddListener(RefreshPublicTopicVibes);
            #endregion
        }
    }

    #region client startup
    [Command]
    public void CmdAskLoadConnectingScene() //test to see if messages get passed between server<-->client and scene changes on server
    {
        RpcClientLoadConnectingScene();
    }

    [Command]
    public void CmdAskLoadMainMenuScene()
    {
        RpcClientLoadMainMenuScene();
    }

    [ClientRpc]
    public void RpcClientLoadConnectingScene()
    {
        SceneManager.LoadScene("Connecting");
    }

    [ClientRpc]
    public void RpcClientLoadMainMenuScene()
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
        {
            SceneManager.LoadScene("MainMenu");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
            StartCoroutine(ClientConnected());
    }
    #endregion

    #region demo
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
    #endregion
}
