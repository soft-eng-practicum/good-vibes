using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Security.Cryptography;
using DigiWar.Security.Cryptography;
using System.Text;
using UnityEngine.SceneManagement;

public class RegistrationController : MonoBehaviour
{
    [SerializeField] InputField clawmailField;
    [SerializeField] InputField passwordField;
    [SerializeField] Button registerBtn;

    MainMenuController mmc;

    private string salt;
    private string hash;

    /*public override void OnStartLocalPlayer()
    {
        print("registrationcontroller client");
    }*/

    private void Update()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            mmc = GetComponent<MainMenuController>();
            clawmailField = GameObject.Find("ClawmailField").GetComponent<InputField>();
            passwordField = GameObject.Find("PasswordField").GetComponent<InputField>();
            registerBtn = GameObject.Find("SubmitRegister").GetComponent<Button>();
        }
    }

    public void CallRegister()
    {
        StartCoroutine(Register());
    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("clawmail", clawmailField.text);
        form.AddField("hash", hash);
        form.AddField("salt", salt);
        WWW www = new WWW("http://localhost/sql/register.php", form);
        yield return www;
        
        if (www.text == "0") //no errors
        {
            string update = "User created successfully.";
            Debug.Log(update);
            //mmc show user create successfully, close register panel and open login panel
            StartCoroutine(ShowRegistrationUpdate(update));
        }
        else
        {
            string update = "User creation failed. Error #" + www.text;
            Debug.Log(update);
            //mmc tell user there was an error
            StartCoroutine(ShowRegistrationUpdate(update));
        }
    }

    IEnumerator ShowRegistrationUpdate(string update)
    {
        registerBtn.transform.GetChild(0).GetComponent<Text>().text = update;
        yield return new WaitForSeconds(3f);
        registerBtn.transform.GetChild(0).GetComponent<Text>().text = "Submit";
    }

    public void VerifyInputs()
    {
        if (clawmailField.text.Contains("@ggc.edu"))
            clawmailField.text.Remove((clawmailField.text.IndexOf('@') + 7));

        SHA512 sha = new SHA512Managed();
        sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes("ourlordandsaviorkirby" + clawmailField.text));

        byte[] saltByte = sha.Hash/*SHA512.Create("ourlordandsaviorkirby" + clawmailField.text).Hash*/ ;
        StringBuilder strBuilder = new StringBuilder();
        for (int i = 0; i < saltByte.Length; i++)
        {
            strBuilder.Append(saltByte[i].ToString("x2"));
        }
            salt = strBuilder.ToString();
        hash = UnixCrypt.Crypt(salt, passwordField.text); //HashAlgorithm.Create();

        registerBtn.interactable = (clawmailField.text.Contains("@ggc.edu") && passwordField.text.Length >= 8);
    }
}
