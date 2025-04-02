using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DBManager : MonoBehaviour
{
    public static DBManager Instance { get; private set; }

    string checkHighScoreURL = "http://localhost/check_highscore.php";
    string updateHighScoreURL = "http://localhost/update_highscore.php";
    string insertHighScoreURL = "http://localhost/insert_highscore.php";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartCheckHighScore(string userId, int musicId, int newScore, int newCombo, float newAccuracy, string newRate)
    {
        StartCoroutine(CheckHighScore(userId, musicId, newScore, newCombo, newAccuracy, newRate));
        Debug.Log("��� �ڷ�ƾ");
    }

    IEnumerator CheckHighScore(string userId, int musicId, int newScore, int newCombo, float newAccuracy, string newRate)
    {
        Debug.Log("StartCheckHighScore() ȣ���!");
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("musicId", musicId);
        form.AddField("musicScore", newScore);

        UnityWebRequest www = UnityWebRequest.Post(checkHighScoreURL, form);
        yield return www.SendWebRequest();
        Debug.Log("22");
        if (www.result == UnityWebRequest.Result.Success)
        {
            string responseText = www.downloadHandler.text.Trim().ToLower();
            Debug.Log(www.downloadHandler.text);
            if (www.downloadHandler.text == "newHighScore")
            {
                Debug.Log("���ο� �ְ���");
                StartUpdateHighScore(userId, musicId, newScore, newCombo, newAccuracy, newRate);
            }
            else if(www.downloadHandler.text == "newRecord")
            {
                Debug.Log("���� ����� ��� ���� ����");
                StartInsertHighScore(userId, musicId, newScore, newCombo, newAccuracy, newRate);
            }
            else if (www.downloadHandler.text == "fail")
            {
                Debug.Log("���� �ְ� ��� ������");
            }
        }
        else
        {
            Debug.LogError("�ְ� ��� Ȯ�� ����: " + www.error);
        }
    }

    public void StartUpdateHighScore(string userId, int musicId, int newScore, int newCombo, float newAccuracy, string newRate)
    {
        StartCoroutine(UpdateHighScore(userId, musicId, newScore, newCombo, newAccuracy, newRate));
    }

    IEnumerator UpdateHighScore(string userId, int musicId, int newScore, int newCombo, float newAccuracy, string newRate)
    {
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("musicId", musicId.ToString());
        form.AddField("musicScore", newScore.ToString());
        form.AddField("musicCombo", newCombo.ToString());
        form.AddField("musicAccuracy", newAccuracy.ToString("F2"));
        form.AddField("musicRate", newRate);

        UnityWebRequest www = UnityWebRequest.Post(updateHighScoreURL, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("�ְ� ��� ���� ����");
        }
        else
        {
            Debug.LogError("�ְ� ��� ���� ����: " + www.error);
        }
    }

    public

    void StartInsertHighScore(string userId, int musicId, int newScore, int newCombo, float newAccuracy, string newRate)
    {
        StartCoroutine(InsertHighScore(userId, musicId, newScore, newCombo, newAccuracy, newRate));
    }

    IEnumerator InsertHighScore(string userId, int musicId, int newScore, int newCombo, float newAccuracy, string newRate)
    {
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("musicId", musicId.ToString());
        form.AddField("musicScore", newScore.ToString());
        form.AddField("musicCombo", newCombo.ToString());
        form.AddField("musicAccuracy", newAccuracy.ToString("F2"));
        form.AddField("musicRate", newRate);

        UnityWebRequest www = UnityWebRequest.Post(insertHighScoreURL, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("�ְ� ��� ���� ����");
        }
        else
        {
            Debug.LogError("�ְ� ��� ���� ����: " + www.error);
        }
    }
}