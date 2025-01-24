using UnityEngine;

public class GameManager_PSH : MonoBehaviour
{
    #region ManagerObject
    public static GameManager_PSH GM_Instance;
    public static GameManager_PSH Instance { get { Init(); return GM_Instance; } }

    InputManager M_Input = new InputManager();
    public static InputManager Input { get { return Instance.M_Input; } }

    ResourceManager M_Resource = new ResourceManager();

    public static ResourceManager Resource { get { return Instance.M_Resource; } }

    BMSManager M_BMS = new BMSManager();
    public static BMSManager BMS { get { return Instance.M_BMS; } }
    #endregion

    static void Init()
    {
        if (GM_Instance == null)
        {
            GameObject gmObject = GameObject.Find("GameManager_PSH");
            if (gmObject == null)
            {
                gmObject = new GameObject { name = "GameManager_PSH" };
                gmObject.AddComponent<GameManager_PSH>();
                gmObject.AddComponent<MusicData>();
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
}
