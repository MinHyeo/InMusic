using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _instance;

    public static Managers Instance
    {
        get
        {
            if (_instance == null)
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning("[Managers] Application is quitting. Returning null instance.");
                    return null;
                }

                GameObject go = GameObject.Find("Managers");
                if (go == null)
                {
                    // 씬 종료 중에는 생성하지 않도록 방어
                    if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().isLoaded)
                    {
                        Debug.LogWarning("[Managers] Scene is unloading. Not creating new Managers.");
                        return null;
                    }

                    go = new GameObject("Managers");
                    DontDestroyOnLoad(go);
                }

                _instance = go.GetComponent<Managers>();
                if (_instance == null)
                    _instance = go.AddComponent<Managers>();
            }

            return _instance;
        }
    }

    private static bool applicationIsQuitting = false;

    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }


    public UIManager UI { get; private set; }
    public InputManager Input { get; private set; }
    public SoundManager Sound { get; private set; }
    public KeyManager Key { get; private set; } //
    private bool _initialized = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        if (!_initialized)
        {
            InitializeManagers();
            _initialized = true;
        }
    }

    private void InitializeManagers()
    {
        Input = new InputManager();
        Sound = new SoundManager();
        Key = new KeyManager();

        //  UIManager를 GameObject로 만들고 컴포넌트로 붙임
        GameObject uiObj = new GameObject("UIManager");
        uiObj.transform.SetParent(this.transform);
        UI = uiObj.AddComponent<UIManager>();
    }

    // 유틸리티 메서드들
    public T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        return comp != null ? comp : go.AddComponent<T>();
    }

    public T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : Object
    {
        if (!recursive)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform child = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || child.name == name)
                {
                    T comp = child.GetComponent<T>();
                    if (comp != null)
                        return comp;
                }
            }
        }
        else
        {
            foreach (T comp in go.GetComponentsInChildren<T>(true))
            {
                if (string.IsNullOrEmpty(name) || comp.name == name)
                    return comp;
            }
        }

        return null;
    }

    private void Update()
    {
        Managers.Instance.Input.Update();
    }


}