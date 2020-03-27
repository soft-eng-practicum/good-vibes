using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Security.Cryptography;
using DigiWar.Security.Cryptography;
using System.Text;
using UnityEngine.SceneManagement;

public class RegistrationController : NetworkBehaviour
{
    [SerializeField] InputField clawmailField;
    [SerializeField] InputField passwordField;
    [SerializeField] Button registerBtn;

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
            clawmailField = GameObject.Find("ClawmailField").GetComponent<InputField>();
            passwordField = GameObject.Find("PasswordField").GetComponent<InputField>();
            registerBtn = GameObject.Find("SubmitRegister").GetComponent<Button>();
        }
    }

    
    [Command]
    public void CmdCallRegister()
    {
        if (isLocalPlayer)
            return;
        StartCoroutine(Register(clawmailField, hash, salt));
    }

    IEnumerator Register(InputField clawmailField, string hash, string salt)
    {
        WWWForm form = new WWWForm();
        form.AddField("clawmail", clawmailField.text);
        form.AddField("hash", hash);
        form.AddField("salt", salt);
        WWW www = new WWW("http://localhost/sql/register.php", form);
        yield return www;
        Debug.Log("Server connected to DB");

        if (www.text == "0") //no errors
        {
            string update = "User created successfully.";
            Debug.Log(update);
            RpcShowRegistrationUpdate(update);
        }
        else
        {
            string update = "User creation failed. Error #" + www.text;
            Debug.Log(update);
            RpcShowRegistrationUpdate(update);
        }
    }

    [ClientRpc]
    public void RpcShowRegistrationUpdate(string update)
    {;
        StartCoroutine(ShowRegistrationUpdate(update));
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
