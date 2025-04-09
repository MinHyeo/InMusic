using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebManager : MonoBehaviour
{
    string loginURL = "http://localhost/InmusicScripts/login.php";
    string signiupURL = "http://localhost/InmusicScripts/signup.php";
    string checkMusicURL = "http://localhost/InmusicScripts/musiccheck.php";
    //string musicLogURL = "http://localhost/InmusicScripts/getlog.php"; //�����ʿ��� �α��� �� �ٷ� �α� ���� ã����
    //string LogUpdateURL = "http://localhost/InmusicScripts/updatelog.php";

    #region �α��� �� ��� ��������
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
                Debug.Log($"���� ���� �Ф� {www.error}");
            }
            else if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"�������� ���� {www.error}");
            }
            else
            {
                /*�α��� �׽�Ʈ�� �ڵ�
                if (www.downloadHandler.text == $"Login successful, welcome {userName}" ||
                    www.downloadHandler.text == $"User created successfully, welcome  {userName}")
                {
                    Debug.Log("�α��� ����!!");
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    Debug.Log("�α��� ���ФФ�");
                }*/

                string jsonData = www.downloadHandler.text;
                Debug.Log(jsonData);

                List<MusicLog> musicLogs = JsonUtility.FromJson<MusicLogList>(jsonData).GetLogs();

                if (musicLogs == null)
                {
                    Debug.LogError("JSON ��ȯ ����!");
                }
                else
                {
                    foreach (MusicLog log in musicLogs)
                    {
                        Debug.Log($"�α� ���̵�:{log.LogID}");
                    }
                    GameManager_PSH.Data.SetLogDataList(musicLogs);
                }
            }
        }
    }
    #endregion

    #region ���� ��� ������Ʈ�ϱ�

    public void CheckMusic(List<MusicData> data)
    {
        MusicDBList musicDBList = new MusicDBList { musics = new List<MusicDB>()};

        //MusicData => MusicDB�� ��ȯ
        foreach (MusicData mData in data)
        {
            //���� �����ʹ� ����
            if (mData.HasBMS)
            {
                MusicDB tmp = new MusicDB(mData.MusicID, mData.Title, mData.Artist);
                musicDBList.musics.Add(tmp);
            }
        }

        StartCoroutine(CheckMusicToServer(musicDBList));
    }

    IEnumerator CheckMusicToServer(MusicDBList data)
    {
        string jsonData = JsonUtility.ToJson(data);
        WWWForm form = new WWWForm();
        form.AddField("musics", jsonData);
        using (UnityWebRequest www = UnityWebRequest.Post(checkMusicURL, form))
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
                Debug.Log("������ ���� ������ ����ȭ �մϴ�...");
                Debug.Log($"���� ����: {www.downloadHandler.text}");
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

    #region ��� ������Ʈ �ϱ�
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
                Debug.Log($"���� ���� �Ф� {www.error}");
            }
            else if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"�������� ���� {www.error}");
            }
            else
            {
                if (www.downloadHandler.text == $"Log updated successfully")
                {
                    Debug.Log("��� ���� ����");
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    Debug.Log("��� ���� ���� ����");
                }
            }
        }
    }*/
    #endregion
}
