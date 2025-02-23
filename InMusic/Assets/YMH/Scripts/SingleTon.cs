using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    public static bool HasInstance => _instance != null;
    public static T TryGetInstance() => HasInstance ? _instance : null;
    public static T Current => _instance;

    //싱글톤
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                if (SceneManager.GetActiveScene().name != "YMH")
                {
                    Debug.LogWarning("PlayManager는 PlayScene에서만 생성됩니다.");
                    return null;
                }

                _instance = FindAnyObjectByType<T>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name + "_AutoCreated";
                    obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        InitializeSingleton();
    }

    protected virtual void InitializeSingleton()
    {
        if (!Application.isPlaying)
            return;

        _instance = this as T;
    }
}