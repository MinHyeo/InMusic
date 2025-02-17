using Play;
using System;
using System.Collections;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region ManagerObject
    public static GameManager GM_Instance;
    public static GameManager Instance { get { Init(); return GM_Instance; } }

    InputManager M_Input = new InputManager();
    public static InputManager Input { get { return Instance.M_Input; } }

    ResourceManager M_Resource = new ResourceManager();
    public static ResourceManager Resource { get { return Instance.M_Resource; } }
    #endregion

    private Song songTitle;

    static void Init()
    {
        if (GM_Instance == null)
        {
            GameObject gmObject = GameObject.Find("GameManager");
            if (gmObject == null)
            {
                gmObject = new GameObject { name = "GameManager" };
                gmObject.AddComponent<GameManager>();
            }
            DontDestroyOnLoad(gmObject);
            GM_Instance = gmObject.GetComponent<GameManager>();

            GM_Instance.M_Input.Init();
        }
    }

    void Update()
    {
        M_Input.UIUpdate();
        M_Input.NoteUpdate();
    }

    public void StartGame(string SongTitle)
    {
        Enum.TryParse(SongTitle, out songTitle);
        SceneManager.sceneLoaded += OnPlaySceneLoaded;  //������ ����

        SceneManager.LoadScene("YMH");
    }

    private void OnPlaySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "YMH")
        {
            StartCoroutine(WaitForPlayManagerAndStartGame());
            SceneManager.sceneLoaded -= OnPlaySceneLoaded; // �̺�Ʈ ���� ����
        }
    }

    public IEnumerator WaitForPlayManagerAndStartGame()
    {
        // PlayManager가 존재할 때까지 대기
        while (PlayManager.Instance == null)
        {
            yield return null;  // 다음 프레임까지 대기
        }

        // PlayManager가 초기화되면 메서드 호출
        //PlayManager.Instance.StartGame(songTitle);
    }

    public void SelectSong(Song songTitle)
    {
        Debug.Log("Try Scene Load");
        SceneManager.LoadScene("test_SSW");
    }

    public void ReturnMusicSelectScene(ScoreData saveData)
    {
        //발동 조건 : 결과창에서 next 버튼을 클릭시 작동

        //Unity에 저장

        //저장 끝나면 씬 이동
        // 1) JSON 저장
        SavePlayData savePlayData = FindFirstObjectByType<SavePlayData>();
        if (savePlayData != null)
        {
            // scoreData 저장 (이미 "Heya" 등 곡 이름이 들어있다고 가정)
            savePlayData.SaveSongScore(saveData);
            Debug.Log($"[GameManager] 저장 완료: {saveData.songName}, 점수: {saveData.score}");
        }
        else
        {
            Debug.LogWarning("[GameManager] SavePlayData 객체를 찾지 못했습니다. 점수를 저장할 수 없습니다.");
        }

        // 2) 저장 완료 후, 곡 선택 씬 등 원하는 씬으로 이동
        SceneManager.LoadScene("test_SSW");
    }
}
