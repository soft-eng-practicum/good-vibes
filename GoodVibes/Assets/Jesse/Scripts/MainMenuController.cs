using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    PlayerController localPlayer;
    //NetworkConnection localNC;

    public GameObject loginRegisterPanels;
    public GameObject loginPanel;
    public GameObject loginBtn;
    public GameObject registerPanel;
    public GameObject registerBtn;

    public GameObject mainMenuPanels;
    public GameObject publicTopicVibesPanel;
    public GameObject publicTopicVibesBtn;
    public GameObject personalTopicVibesPanel;
    public GameObject personalTopicVibesBtn;
    public GameObject postTopicVibeBtn;
    public GameObject postTopicVibePanel;
    public GameObject legalAgreementPanel;
    public GameObject legalAgreementBtn;
    public GameObject legalAgreementSb;

    public GameObject testText;
    bool checkAgreement;

    public InputField replyInputField;
    public InputField sendTopicSubjectInputField;
    public InputField sendTopicMessageInputField;
    public InputField loginClawmailField;
    public InputField loginPasswordField;
    public InputField registerClawmailField;
    public InputField registerPasswordField;
    public Dropdown userType;

    private void Start()
    {
        OpenLoginRegisterPanels();
        checkAgreement = false;
    }

    private void Update()
    {
        if (loginRegisterPanels.activeSelf)
        {
            if (loginPanel.activeSelf)
                loginBtn.GetComponent<Image>().color = loginBtn.GetComponent<Button>().colors.pressedColor;
            else
                loginBtn.GetComponent<Image>().color = loginBtn.GetComponent<Button>().colors.normalColor;

            if (registerPanel.activeSelf)
                registerBtn.GetComponent<Image>().color = registerBtn.GetComponent<Button>().colors.pressedColor;
            else
                registerBtn.GetComponent<Image>().color = registerBtn.GetComponent<Button>().colors.normalColor;
        }
        else if (mainMenuPanels.activeSelf)
        {
            if (publicTopicVibesPanel.activeSelf)
                publicTopicVibesBtn.GetComponent<Image>().color = publicTopicVibesBtn.GetComponent<Button>().colors.pressedColor;
            else
                publicTopicVibesBtn.GetComponent<Image>().color = publicTopicVibesBtn.GetComponent<Button>().colors.normalColor;

            if (personalTopicVibesPanel.activeSelf)
                personalTopicVibesBtn.GetComponent<Image>().color = personalTopicVibesBtn.GetComponent<Button>().colors.pressedColor;
            else
                personalTopicVibesBtn.GetComponent<Image>().color = personalTopicVibesBtn.GetComponent<Button>().colors.normalColor;

            if (postTopicVibePanel.activeSelf)
                postTopicVibeBtn.GetComponent<Image>().color = postTopicVibeBtn.GetComponent<Button>().colors.pressedColor;
            else
                postTopicVibeBtn.GetComponent<Image>().color = postTopicVibeBtn.GetComponent<Button>().colors.normalColor;
        }

        if (checkAgreement)
        {
            if (legalAgreementSb.GetComponent<Scrollbar>().value == 0)
            {
                legalAgreementBtn.GetComponent<Button>().interactable = true;
                checkAgreement = false;
            }
        }
    }

    public void SetLocalPlayer(PlayerController set)
    {
        localPlayer = set;
    }

    void OpenLoginRegisterPanels()
    {
        loginRegisterPanels.SetActive(true);
        SetRegisterPanel(false);
        SetLoginPanel(false);
        mainMenuPanels.SetActive(false);
        publicTopicVibesPanel.SetActive(false);
    }

    public void CloseLoginRegisterPanels()
    {
        loginRegisterPanels.SetActive(false);
        SetRegisterPanel(false);
        SetLoginPanel(false);
    }

    public void ToggleRegisterPanel()
    {
        if (registerPanel.activeSelf)
            registerPanel.SetActive(false);
        else if (!loginPanel.activeSelf)
            registerPanel.SetActive(true);
        else if (loginPanel.activeSelf)
        {
            ToggleLoginPanel();
            registerPanel.SetActive(true);
        }
    }

    void SetRegisterPanel(bool set)
    {
        registerPanel.SetActive(set);
    }

    public void ToggleLoginPanel()
    {
        if (loginPanel.activeSelf)
            loginPanel.SetActive(false);
        else if (!registerPanel.activeSelf)
            loginPanel.SetActive(true);
        else if (registerPanel.activeSelf)
        {
            ToggleRegisterPanel();
            loginPanel.SetActive(true);
        }
    }

    void SetLoginPanel(bool set)
    {
        loginPanel.SetActive(set);
    }

    public void VerifyInputs()
    {
        localPlayer.VerifyInputs();
    }

    public void OpenMainMenuOptions(string username)
    {
        mainMenuPanels.SetActive(true);
        mainMenuPanels.transform.GetChild(1).GetComponent<Text>().text = "Welcome, " + username + "!";
        publicTopicVibesPanel.SetActive(false);
        personalTopicVibesPanel.SetActive(false);
        postTopicVibePanel.SetActive(false);
        legalAgreementPanel.SetActive(false);
    }

    public void TogglePublicTopicVibesPanel()
    {
        if (publicTopicVibesPanel.activeSelf)
            publicTopicVibesPanel.SetActive(false);
        else if (!personalTopicVibesPanel.activeSelf && !postTopicVibePanel.activeSelf) //other main menu panels will go here
            publicTopicVibesPanel.SetActive(true);
        else if (personalTopicVibesPanel.activeSelf) 
        {
            TogglePersonalTopicVibesPanel();
            publicTopicVibesPanel.SetActive(true);
        }
        else if (postTopicVibePanel.activeSelf)
        {
            TogglePostTopicVibesPanel();
            publicTopicVibesPanel.SetActive(true);
        }
    }

    public void TogglePersonalTopicVibesPanel()
    {
        if (personalTopicVibesPanel.activeSelf)
            personalTopicVibesPanel.SetActive(false);
        else if (!publicTopicVibesPanel.activeSelf && !postTopicVibePanel.activeSelf) //other main menu panels (checking if false) will go here
            personalTopicVibesPanel.SetActive(true);
        else if (publicTopicVibesPanel.activeSelf) 
        {
            TogglePublicTopicVibesPanel();
            personalTopicVibesPanel.SetActive(true);
        }
        else if (postTopicVibePanel.activeSelf)
        {
            TogglePostTopicVibesPanel();
            personalTopicVibesPanel.SetActive(true);
        }
    }

    public void TogglePostTopicVibesPanel()
    {
        //ClearInput();
        if (postTopicVibePanel.activeSelf)
            postTopicVibePanel.SetActive(false);
        else if (!publicTopicVibesPanel.activeSelf && !personalTopicVibesPanel.activeSelf) //other main menu panels (checking if false) will go here
            postTopicVibePanel.SetActive(true);
        else if (publicTopicVibesPanel.activeSelf) //add every other main menu panel to its own else if
        {
            TogglePublicTopicVibesPanel();
            postTopicVibePanel.SetActive(true);
        }
        else if (personalTopicVibesPanel.activeSelf)
        {
            TogglePersonalTopicVibesPanel();
            postTopicVibePanel.SetActive(true);
        }
    }

    public void OpenLegalAgreementPanel()
    {
        print("openlegal");
        legalAgreementPanel.SetActive(true);
        legalAgreementBtn.GetComponent<Button>().interactable = false;
        checkAgreement = true;
    }

    public void Agree()
    {
        legalAgreementPanel.SetActive(false);
        localPlayer.Agree();
    }

    public void ClearInput()
    {
        if (EventSystem.current.currentSelectedGameObject.name == "SubmitLogin" || EventSystem.current.currentSelectedGameObject.name == "LoginPanelBtn")
        {
            loginClawmailField.text = "";
            loginPasswordField.text = "";
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "SubmitRegister" || EventSystem.current.currentSelectedGameObject.name == "RegisterPanelBtn")
        {
            registerClawmailField.text = "";
            registerPasswordField.text = "";
            userType.value = 0;
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "PostMsg" || EventSystem.current.currentSelectedGameObject.name == "GetPosts")
            replyInputField.text = "";
        else if (EventSystem.current.currentSelectedGameObject.name == "PostTopicMsg" || EventSystem.current.currentSelectedGameObject.name == "PostTopicVibePanelBtn")
        {
            sendTopicMessageInputField.text = "";
            sendTopicSubjectInputField.text = "";
        }
    }

    public void VerifyLength()
    {
        if (GameObject.Find("ReplyCanvas") != null && GameObject.Find("ReplyCanvas").activeSelf == true)
            GameObject.Find("PostMsg").GetComponent<Button>().interactable = (replyInputField.text.Length > 0);
        if (postTopicVibePanel.activeSelf == true)
            GameObject.Find("PostTopicMsg").GetComponent<Button>().interactable = (sendTopicMessageInputField.text.Length > 0 && sendTopicSubjectInputField.text.Length > 0);
    }
}