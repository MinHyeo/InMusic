using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManagerProvider
{
    private static IGameManager _gameManager;

    public static IGameManager Instance
    {
        get
        {
            // 캐시된 객체가 없거나 이미 파괴된 경우 다시 찾기
            if (_gameManager == null || (_gameManager is Object unityObj && unityObj == null))
            {
                if (GameManager.Instance != null)
                    _gameManager = GameManager.Instance;
                else if (KGB_GameManager_Multi.Instance != null)
                    _gameManager = KGB_GameManager_Multi.Instance;

                Debug.Log($"[GameManagerProvider] (Re)initialized: {_gameManager}");
            }

            return _gameManager;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void SetupSceneReset()
    {
        SceneManager.sceneLoaded += (_, __) =>
        {
            _gameManager = null;
            Debug.Log("[GameManagerProvider] Cleared _gameManager on scene load (SceneManager.sceneLoaded)");
        };
    }
}
