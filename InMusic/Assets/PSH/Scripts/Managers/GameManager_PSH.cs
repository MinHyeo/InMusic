using UnityEngine;

public class GameManager_PSH : MonoBehaviour
{
    string selectedMusic;
    static GameObject musicDataRoot;
    [SerializeField] bool isSteamCheck = false;
    [SerializeField] bool isBackendCheck = false;


    public GameObject DataRoot { get { return musicDataRoot; } }

    #region ManagerObject
    public static GameManager_PSH GM_Instance;
    public static GameManager_PSH Instance { get { Init(); return GM_Instance; } }

    InputManager_PSH M_Input = new InputManager_PSH();
    public static InputManager_PSH Input { get { return Instance.M_Input; } }

    ResourceManager M_Resource = new ResourceManager();

    public static ResourceManager Resource { get { return Instance.M_Resource; } }

    BMSManager M_BMS = new BMSManager(); //여기 문제 있음
    public static BMSManager BMS { get { return Instance.M_BMS; } }

    DataManager M_Data = new DataManager();
    public static DataManager Data { get { return Instance.M_Data; } }

    static WebManager M_Web = null;
    public static WebManager Web { get {if (M_Web == null) Init(); return M_Web; } 
                                   private set { M_Web = value; } }

    /* SteamManager는 독립적으로 다뤄야할 듯...
    static SteamManager M_Steam = null;
    public static SteamManager Steam { get { if (M_Steam == null) Init(); return Steam; }
                                      private set { M_Steam = value; }}
    */

    #endregion

    public static bool SteamCheck { get { return Instance.isSteamCheck; } set { Instance.isSteamCheck = value; } }
    public static bool BackendCheck { get { return Instance.isBackendCheck; } set { Instance.isBackendCheck = value; } }

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
                gmObject.AddComponent<WebManager>();
                gmObject.AddComponent<Player>();
            }
            DontDestroyOnLoad(gmObject);

            GM_Instance = gmObject.GetComponent<GameManager_PSH>();

            //매니저들 초기화
            Web = gmObject.GetComponent<WebManager>();
            //GM_Instance.M_Input.Init();
            GM_Instance.M_Data.Init();

            musicDataRoot = new GameObject("MusicDataRoot");
            DontDestroyOnLoad(musicDataRoot);
        }
    }

    void Update()
    {
        /*
        M_Input.UIUpdate();
        M_Input.NoteUpdate();
        */
    }

    //by KGB. 선택한 노래정보를 씬 이동후에도 유지(Single_Lobby_UI와 연계)
    public void SetSelectedMusic(string musicName)
    {
        selectedMusic = musicName;
    }
    public string GetSelectedMusic()
    {
        return selectedMusic;
    }
}
