﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MainMenuController : MonoBehaviour
{
    public GameObject loginRegisterPanels;
    public GameObject loginPanel;
    public GameObject loginBtn;
    public GameObject registerPanel;
    public GameObject registerBtn;

    public GameObject mainMenuPanels;
    public GameObject publicTopicVibesPanel;
    public GameObject publicTopicVibesBtn;
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

    public void OpenMainMenuOptions(string username)
    {
        mainMenuPanels.SetActive(true);
        mainMenuPanels.transform.GetChild(1).GetComponent<Text>().text = "Welcome, " + username + "!";
    }

    public void TogglePublicTopicVibesPanel()
    {
        if (publicTopicVibesPanel.activeSelf)
            publicTopicVibesPanel.SetActive(false);
        else /*if (!registerPanel.activeSelf)*/ //other main menu panels will go here
            publicTopicVibesPanel.SetActive(true);
        /*else if (registerPanel.activeSelf)
        {
            ToggleRegisterPanel();
            SetLoginPanel(true);
        }*/
    }
}