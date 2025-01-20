using UnityEngine;

public class GameManager_PSH : MonoBehaviour
{
    string selectedMusic;

    #region ManagerObject
    public static GameManager_PSH GM_Instance;
    public static GameManager_PSH Instance { get { Init(); return GM_Instance; } }

    InputManager M_Input = new InputManager();
    public static InputManager Input { get { return Instance.M_Input; } }

    ResourceManager M_Resource = new ResourceManager();
    public static ResourceManager Resource { get { return Instance.M_Resource; } }
    #endregion

    static void Init()
    {
        if (GM_Instance == null)
        {
            GameObject gmObject = GameObject.Find("GameManager");
            if (gmObject == null)
            {
                gmObject = new GameObject { name = "GameManager" };
                gmObject.AddComponent<GameManager_PSH>();
            }
            DontDestroyOnLoad(gmObject);
            GM_Instance = gmObject.GetComponent<GameManager_PSH>();

            GM_Instance.M_Input.Init();
        }
    }

    void Update()
    {
        M_Input.UIUpdate();
        M_Input.NoteUpdate();
    }

    //by KGB. ������ �뷡������ �� �̵��Ŀ��� ����(Single_Lobby_UI�� ����)
    public void SetSelectedMusic(string musicName)
    {
        selectedMusic = musicName;
    }
    public string GetSelectedMusic()
    {
        return selectedMusic;
    }
}
