using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebManager : MonoBehaviour
{
    string loginURL = "http://localhost/InmusicScripts/login.php";
    string signiupURL = "http://localhost/InmusicScripts/signup.php";
    string musicLogURP = "http://localhost/InmusicScripts/getlog.php";

    #region �α���
    public void UserLogin(string userID, string userName){
        StartCoroutine(LoginToServer(userID, userName));
    }

    IEnumerator LoginToServer(string userID, string userName)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", userID);
        form.AddField("userName", userName);
        using (UnityWebRequest www = UnityWebRequest.Post(loginURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log($"���� ���� �Ф� {www.error}");
            }
            else if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"�������� ���� {www.error}");
            }
            else
            {
                if (www.downloadHandler.text == $"Login successful, welcome {userName}" ||
                    www.downloadHandler.text == $"User created successfully, welcome  {userName}")
                {
                    Debug.Log("�α��� ����!!");
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    Debug.Log("�α��� ���ФФ�");
                }
            }
        }
    }
    #endregion

    #region ȸ�� ����
    /// <summary>
    /// ȸ�� ���� �׽�Ʈ�� �޼���
    /// </summary>
    /// <param name="newUserID">���� ID</param>
    /// <param name="newUserName">���� �г���</param>
    public void UserSignUp(string newUserID, string newUserName)
    {
        StartCoroutine(SignUpToServer(newUserID, newUserName));
    }

    IEnumerator SignUpToServer(string newID, string newName)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", newID);
        form.AddField("userName", newName);
        using (UnityWebRequest www = UnityWebRequest.Post(signiupURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log($"���� ���� �Ф� {www.error}");
            }
            else if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"�������� ���� {www.error}");
            }
            else
            {
                if (www.downloadHandler.text == $"User created successfully")
                {
                    Debug.Log("ȸ������ ����!!");
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    Debug.Log("ȸ������ ����");
                }
            }
        }
    }
    #endregion
}
