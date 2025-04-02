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
        Debug.Log("기록 코루틴");
    }

    IEnumerator CheckHighScore(string userId, int musicId, int newScore, int newCombo, float newAccuracy, string newRate)
    {
        Debug.Log("StartCheckHighScore() 호출됨!");
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
                Debug.Log("새로운 최고기록");
                StartUpdateHighScore(userId, musicId, newScore, newCombo, newAccuracy, newRate);
            }
            else if(www.downloadHandler.text == "newRecord")
            {
                Debug.Log("기존 기록이 없어서 새로 만듬");
                StartInsertHighScore(userId, musicId, newScore, newCombo, newAccuracy, newRate);
            }
            else if (www.downloadHandler.text == "fail")
            {
                Debug.Log("기존 최고 기록 못넘음");
            }
        }
        else
        {
            Debug.LogError("최고 기록 확인 실패: " + www.error);
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
            Debug.Log("최고 기록 갱신 성공");
        }
        else
        {
            Debug.LogError("최고 기록 갱신 실패: " + www.error);
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
            Debug.Log("최고 기록 생성 성공");
        }
        else
        {
            Debug.LogError("최고 기록 생성 실패: " + www.error);
        }
    }
}