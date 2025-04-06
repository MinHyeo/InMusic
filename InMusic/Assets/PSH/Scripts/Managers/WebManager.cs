using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebManager : MonoBehaviour
{
    string loginURL = "http://localhost/InmusicScripts/login.php";
    string signiupURL = "http://localhost/InmusicScripts/signup.php";
    //string musicLogURL = "http://localhost/InmusicScripts/getlog.php"; //서버쪽에서 로그인 후 바로 로그 내용 찾아줌
    //string LogUpdateURL = "http://localhost/InmusicScripts/updatelog.php";

    #region 로그인
    public void UserLogin(string userID, string userName){
        StartCoroutine(LoginToServer(userID, userName));
    }

    IEnumerator LoginToServer(string userID, string userName, string logID = "all")
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", userID);
        form.AddField("userName", userName);
        form.AddField("logID",logID);
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
                /*로그인 테스트용 코드
                if (www.downloadHandler.text == $"Login successful, welcome {userName}" ||
                    www.downloadHandler.text == $"User created successfully, welcome  {userName}")
                {
                    Debug.Log("로그인 성공!!");
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    Debug.Log("로그인 실패ㅠㅠ");
                }*/

                string jsonData = www.downloadHandler.text;
                Debug.Log(jsonData);

                List<MusicLog> musicLogs = JsonUtility.FromJson<MusicLogList>(jsonData).GetLogs();

                if (musicLogs == null)
                {
                    Debug.LogError("JSON 변환 실패!");
                }
                else
                {
                    foreach (MusicLog log in musicLogs)
                    {
                        Debug.Log($"음악 번호:{log.LogID}");
                    }
                    GameManager_PSH.Data.SetLogDataList(musicLogs);
                }
            }
        }
    }
    #endregion

    #region 회원 가입 및 기록 가져오기
    /// <summary>
    /// 회원 가입 테스트용 메서드
    /// </summary>
    /// <param name="newUserID">스팀 ID</param>
    /// <param name="newUserName">스팀 닉네임</param>
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
    #endregion

    #region 기록 업데이트 하기
    /*
    public void UpdateLog(MusicLog newLog, string userID = "76561198365750763")
    {
        StartCoroutine(UpdateLogServer(newLog, userID));
    }

    IEnumerator UpdateLogServer(MusicLog newLog, string userID = "76561198365750763")
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", "76561198365750763");
        form.AddField("musicID", "4");
        form.AddField("logID", newLog.LogID);
        form.AddField("score", newLog.Score);
        form.AddField("accuracy", newLog.Accuracy);
        form.AddField("combo", newLog.Combo);
        form.AddField("rank", newLog.Rank);
        using (UnityWebRequest www = UnityWebRequest.Post(LogUpdateURL, form))
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
                if (www.downloadHandler.text == $"Log updated successfully")
                {
                    Debug.Log("기록 저장 성공");
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    Debug.Log("기록 저장 실패 실패");
                }
            }
        }
    }*/
    #endregion

}
