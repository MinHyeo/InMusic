using Fusion;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class PasswordPopup : MonoBehaviour
{
    [SerializeField]
    private InputField passwordInputField;
    [SerializeField]
    private GameObject passwrodFailUI;

    private Button confirmButton;
    private Button cancelButton;

    private SessionInfo sessionInfo;

    public void SetSessionInfo(SessionInfo sessionInfo)
    {
        this.sessionInfo = sessionInfo;

        passwordInputField.text = "";
        passwrodFailUI.SetActive(false);
    }

    public void OnConfirmButton()
    {
        string enteredPwd = passwordInputField.text;
        string enteredPwdHash = HashUtils.GetSha256(enteredPwd);

        string correctPwdHash = sessionInfo.Properties["password"];

        if (correctPwdHash == enteredPwdHash)
        {
            // Close the password popup
            gameObject.SetActive(false);

            // Join the room
            LobbyManager.Instance.TryJoinRoom(sessionInfo.Name);
        }
        else
        {
            // Show password fail UI
            passwrodFailUI.SetActive(true);
            gameObject.GetComponentInChildren<Animator>().SetTrigger("Fail");
        }
    }

    public void OnCancelButton()
    {
        // Clear the password input field
        passwordInputField.text = "";

        // Hide the password fail UI
        passwrodFailUI.SetActive(false);

        // Close the password popup
        gameObject.SetActive(false);
    }
}