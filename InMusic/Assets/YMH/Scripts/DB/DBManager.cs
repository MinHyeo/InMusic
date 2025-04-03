using Play;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class DBManager : SingleTon<DBManager>
{
    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }

    #region DB���� �÷��� ��� Ȯ�� �� ������ �ֱ�
    public void InsertPlayData(ScoreData scoreData)
    {
        StartCoroutine(InsertPlayDataInDB(scoreData));
    }

    private IEnumerator InsertPlayDataInDB(ScoreData scoreData)
    {
        //DB ���
        string serverPath = "http://localhost/inmusic/MusicLog.php";

        //�� ����
        WWWForm form = new WWWForm();
        form.AddField("steam_id", YMH.SteamManager.Instance.steamId.ToString());
        form.AddField("music_name", scoreData.songName);
        form.AddField("music_score", scoreData.score);
        form.AddField("music_combo", scoreData.maxCombo);
        form.AddField("music_accuracy", scoreData.accuracy.ToString("F2"));
        form.AddField("music_rate", scoreData.rank);

        //������ ������ ����
        using (UnityWebRequest webRequest = UnityWebRequest.Post(serverPath, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"���� ����: {webRequest.error}");
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
    }

    #endregion
}
