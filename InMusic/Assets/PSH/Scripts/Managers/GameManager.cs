using Play;
using System;
using System.Collections;
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

    private IEnumerator WaitForPlayManagerAndStartGame()
    {
        // PlayManager가 존재할 때까지 대기
        while (PlayManager.Instance == null)
        {
            yield return null;  // 다음 프레임까지 대기
        }

        Debug.Log("dddd");
        // PlayManager가 초기화되면 메서드 호출
        PlayManager.Instance.StartGame(songTitle);
    }

    public void SelectSong(Song songTitle)
    {
        Debug.Log("Try Scene Load");
        SceneManager.LoadScene("test_SSW");
    }
}
