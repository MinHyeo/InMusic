using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebManager : MonoBehaviour
{
    string loginURL = "http://localhost/InmusicScripts/login.php";
    string signiupURL = "http://localhost/InmusicScripts/signup.php";

    public void UserLogin(string userID, string userName){
        StartCoroutine(LoginToServer(userID, userName));
    }

    public void UserSignUp(string newUserID, string newUserName)
    {
        StartCoroutine(SignUpToServer(newUserID, newUserName));
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
                Debug.Log($"연결 실패 ㅠㅠ {www.error}");
            }
            else if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"프로토콜 문제 {www.error}");
            }
            else
            {
                if (www.downloadHandler.text == $"Login successful, welcome {userName}")
                {
                    Debug.Log("로그인 성공!!");
                }
                else if (www.downloadHandler.text == $"No users found")
                {
                    Debug.Log("없는 사람임");
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    Debug.Log("로그인 실패ㅠㅠ");
                }
            }
        }
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
                Debug.Log($"연결 실패 ㅠㅠ {www.error}");
            }
            else if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"프로토콜 문제 {www.error}");
            }
            else
            {
                if (www.downloadHandler.text == $"User created successfully")
                {
                    Debug.Log("회원가입 성공!!");
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    Debug.Log("회원가입 실패");
                }
            }
        }
    }
}
