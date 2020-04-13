using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

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
    public GameObject testText;

    private void Start()
    {
        OpenLoginRegisterPanels();
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
}
